using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Initialize a chat session for a user.
    /// </summary>
    [HttpPost("init/{userId:guid}")]
    [ProducesResponseType(typeof(ChatHistoryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ChatHistoryDto>> Initialize(Guid userId, CancellationToken cancellationToken)
    {
        var history = await _chatService.InitializeChatSessionAsync(userId, cancellationToken);
        return Ok(history);
    }

    /// <summary>
    /// Get chat history for a user.
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(ChatHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChatHistoryDto>> GetHistory(Guid userId, CancellationToken cancellationToken)
    {
        var history = await _chatService.GetChatHistoryAsync(userId, cancellationToken);
        if (history is null)
        {
            return NotFound();
        }

        return Ok(history);
    }

    /// <summary>
    /// Save a chat message.
    /// </summary>
    [HttpPost("message")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<MessageDto>> SaveMessage(MessageDto message, CancellationToken cancellationToken)
    {
        var saved = await _chatService.SaveMessageAsync(message, cancellationToken);
        return CreatedAtAction(nameof(GetHistory), new { userId = saved.SenderId }, saved);
    }
}

