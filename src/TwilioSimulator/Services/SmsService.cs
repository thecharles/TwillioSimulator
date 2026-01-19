using Microsoft.EntityFrameworkCore;
using TwilioSimulator.Data;
using TwilioSimulator.DTOs;
using TwilioSimulator.Models;

namespace TwilioSimulator.Services;

public class SmsService : ISmsService
{
    private readonly SimulatorDbContext _context;
    private readonly ICallbackService _callbackService;
    private readonly ILogger<SmsService> _logger;

    public SmsService(SimulatorDbContext context, ICallbackService callbackService, ILogger<SmsService> logger)
    {
        _context = context;
        _callbackService = callbackService;
        _logger = logger;
    }

    public async Task<Guid> ReceiveMessageAsync(SmsReceiveDto dto)
    {
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.PhoneNumber == dto.From);

        if (conversation == null)
        {
            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                PhoneNumber = dto.From,
                CallbackUrl = dto.CallbackUrl,
                LastMessageAt = DateTime.UtcNow
            };
            _context.Conversations.Add(conversation);
        }
        else
        {
            conversation.CallbackUrl = dto.CallbackUrl;
            conversation.LastMessageAt = DateTime.UtcNow;
        }

        var message = new SmsMessage
        {
            Id = Guid.NewGuid(),
            From = dto.From,
            To = dto.To,
            MessageBody = dto.MessageBody,
            Direction = MessageDirection.Inbound,
            CreatedAt = DateTime.UtcNow,
            ConversationId = conversation.Id
        };

        _context.SmsMessages.Add(message);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Received SMS from {From} to {To}", dto.From, dto.To);

        return message.Id;
    }

    public async Task<List<ConversationDto>> GetConversationsAsync()
    {
        var conversations = await _context.Conversations
            .Include(c => c.Messages)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();

        return conversations.Select(c => new ConversationDto
        {
            Id = c.Id,
            PhoneNumber = c.PhoneNumber,
            LastMessageAt = c.LastMessageAt,
            LastMessagePreview = c.Messages
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault()?.MessageBody.Substring(0, Math.Min(50, c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault()?.MessageBody.Length ?? 0)) ?? ""
        }).ToList();
    }

    public async Task<List<MessageDto>> GetMessagesByPhoneAsync(string phoneNumber)
    {
        var messages = await _context.SmsMessages
            .Where(m => m.Conversation.PhoneNumber == phoneNumber)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            From = m.From,
            To = m.To,
            MessageBody = m.MessageBody,
            Direction = m.Direction.ToString(),
            CreatedAt = m.CreatedAt
        }).ToList();
    }

    public async Task<bool> SendReplyAsync(string phoneNumber, string messageBody)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

        if (conversation == null)
        {
            _logger.LogWarning("Conversation not found for phone number {PhoneNumber}", phoneNumber);
            return false;
        }

        var lastInboundMessage = conversation.Messages
            .Where(m => m.Direction == MessageDirection.Inbound)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefault();

        if (lastInboundMessage == null)
        {
            _logger.LogWarning("No inbound message found for conversation {PhoneNumber}", phoneNumber);
            return false;
        }

        var message = new SmsMessage
        {
            Id = Guid.NewGuid(),
            From = lastInboundMessage.To,
            To = phoneNumber,
            MessageBody = messageBody,
            Direction = MessageDirection.Outbound,
            CreatedAt = DateTime.UtcNow,
            ConversationId = conversation.Id
        };

        _context.SmsMessages.Add(message);
        conversation.LastMessageAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var callbackPayload = new SmsSendDto
        {
            From = message.From,
            To = message.To,
            MessageBody = message.MessageBody
        };

        await _callbackService.SendCallbackAsync(conversation.CallbackUrl, callbackPayload);

        _logger.LogInformation("Sent reply to {PhoneNumber}", phoneNumber);

        return true;
    }
}
