using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectService.Api.Services;

namespace ProjectService.Api.Controllers;

/// <summary>
/// Trò chuyện với AI (Gemini) — backend proxy giữ API key, tự nạp ngữ cảnh người dùng.
///   - POST /api/chat        → gửi tin nhắn, nhận trả lời
///   - GET  /api/chat/status → bot đã cấu hình API key chưa
/// </summary>
[ApiController]
[Authorize]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chat;

    public ChatController(ChatService chat)
    {
        _chat = chat;
    }

    [HttpGet("status")]
    public ActionResult<object> Status()
        => Ok(new { enabled = _chat.IsEnabled });

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Send([FromBody] ChatRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var response = await _chat.SendAsync(request, userId, ct);
        return Ok(response);
    }
}
