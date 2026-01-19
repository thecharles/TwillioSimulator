using TwilioSimulator.DTOs;

namespace TwilioSimulator.Services;

public interface ISmsService
{
    Task<Guid> ReceiveMessageAsync(SmsReceiveDto dto);
    Task<List<ConversationDto>> GetConversationsAsync();
    Task<List<MessageDto>> GetMessagesByPhoneAsync(string phoneNumber);
    Task<bool> SendReplyAsync(string phoneNumber, string messageBody);
}
