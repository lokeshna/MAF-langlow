using System.Text.Json;
using MAF.LangflowBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAF.LangflowBot.Controllers;

[ApiController]
[Route("api/copilotkit")]
public sealed class CopilotKitController : ControllerBase
{
    private readonly ILangflowAgentService _langflowAgentService;

    public CopilotKitController(ILangflowAgentService langflowAgentService)
    {
        _langflowAgentService = langflowAgentService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] JsonElement payload, CancellationToken cancellationToken)
    {
        // Lightweight compatibility shim: extracts last user message and returns assistant text.
        var userMessage = ExtractLastUserMessage(payload);
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            return BadRequest(new { error = "No user message found in payload." });
        }

        var reply = await _langflowAgentService.GetReplyAsync(userMessage, cancellationToken);

        return Ok(new
        {
            message = new
            {
                role = "assistant",
                content = reply
            }
        });
    }

    private static string? ExtractLastUserMessage(JsonElement payload)
    {
        if (!payload.TryGetProperty("messages", out var messages) || messages.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        string? latest = null;
        foreach (var message in messages.EnumerateArray())
        {
            if (message.TryGetProperty("role", out var role) && role.GetString() == "user" &&
                message.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.String)
            {
                latest = content.GetString();
            }
        }

        return latest;
    }
}
