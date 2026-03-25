using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Agents.AI;

namespace MAF.LangflowBot.Services;

public sealed class LangflowAgentService : ILangflowAgentService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly Agent _agent;

    public LangflowAgentService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        // Microsoft Agent Framework object used as the bot orchestration anchor.
        _agent = new Agent("langflow-assistant")
        {
            Description = "Assistant backed by a Langflow flow endpoint"
        };
    }

    public async Task<string> GetReplyAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            return "Please enter a message.";
        }

        var endpoint = _configuration["Langflow:Endpoint"];
        var token = _configuration["Langflow:ApiKey"];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return "Langflow endpoint is not configured.";
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(new
            {
                input_value = userMessage,
                output_type = "chat",
                input_type = "chat",
                session_id = Guid.NewGuid().ToString("N"),
                metadata = new
                {
                    agent = _agent.Name,
                    _agent.Description
                }
            })
        };

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new("Bearer", token);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return $"Langflow call failed: {(int)response.StatusCode} {response.ReasonPhrase}";
        }

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);

        // Handles common Langflow response envelopes.
        if (document.RootElement.TryGetProperty("output", out var outputElement) && outputElement.ValueKind == JsonValueKind.String)
        {
            return outputElement.GetString() ?? "No answer returned.";
        }

        if (document.RootElement.TryGetProperty("outputs", out var outputsElement) && outputsElement.ValueKind == JsonValueKind.Array)
        {
            var first = outputsElement.EnumerateArray().FirstOrDefault();
            if (first.ValueKind != JsonValueKind.Undefined && first.TryGetProperty("outputs", out var nestedOutputs))
            {
                var text = nestedOutputs.EnumerateArray()
                    .SelectMany(o => o.TryGetProperty("results", out var r) ? r.EnumerateObject() : Enumerable.Empty<JsonProperty>())
                    .Select(p => p.Value)
                    .Select(v => v.TryGetProperty("text", out var t) ? t.GetString() : null)
                    .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));

                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text!;
                }
            }
        }

        return "I couldn't parse the Langflow response.";
    }
}
