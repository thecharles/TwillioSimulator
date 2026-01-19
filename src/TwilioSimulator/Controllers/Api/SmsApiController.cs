using Microsoft.AspNetCore.Mvc;
using TwilioSimulator.DTOs;
using TwilioSimulator.Services;

namespace TwilioSimulator.Controllers.Api;

[ApiController]
[Route("api/sms")]
public class SmsApiController : ControllerBase
{
    private readonly ISmsService _smsService;

    public SmsApiController(ISmsService smsService)
    {
        _smsService = smsService;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveSms([FromBody] SmsReceiveDto dto)
    {
        if (string.IsNullOrEmpty(dto.From) || string.IsNullOrEmpty(dto.MessageBody))
        {
            return BadRequest(new { error = "From and MessageBody are required" });
        }

        var messageId = await _smsService.ReceiveMessageAsync(dto);
        return Ok(new { messageId, status = "received" });
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var conversations = await _smsService.GetConversationsAsync();
        return Ok(conversations);
    }

    [HttpGet("messages/{phoneNumber}")]
    public async Task<IActionResult> GetMessages(string phoneNumber)
    {
        var decodedPhone = Uri.UnescapeDataString(phoneNumber);
        var messages = await _smsService.GetMessagesByPhoneAsync(decodedPhone);
        return Ok(messages);
    }

    [HttpPost("reply")]
    public async Task<IActionResult> SendReply([FromBody] SendReplyDto dto)
    {
        if (string.IsNullOrEmpty(dto.PhoneNumber) || string.IsNullOrEmpty(dto.MessageBody))
        {
            return BadRequest(new { error = "PhoneNumber and MessageBody are required" });
        }

        var success = await _smsService.SendReplyAsync(dto.PhoneNumber, dto.MessageBody);

        if (!success)
        {
            return NotFound(new { error = "Conversation not found" });
        }

        return Ok(new { status = "sent" });
    }
}
