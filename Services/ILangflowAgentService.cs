namespace MAF.LangflowBot.Services;

public interface ILangflowAgentService
{
    Task<string> GetReplyAsync(string userMessage, CancellationToken cancellationToken = default);
}
