using MAF.LangflowBot.Models;
using MAF.LangflowBot.Services;
using Microsoft.AspNetCore.Mvc;

namespace MAF.LangflowBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BotController : ControllerBase
{
    private readonly ILangflowAgentService _langflowAgentService;

    public BotController(ILangflowAgentService langflowAgentService)
    {
        _langflowAgentService = langflowAgentService;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        var reply = await _langflowAgentService.GetReplyAsync(request.Message, cancellationToken);
        return Ok(new ChatResponse { Reply = reply });
    }
}
