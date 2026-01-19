namespace TwilioSimulator.Models;

public class SmsMessage
{
    public Guid Id { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string MessageBody { get; set; } = string.Empty;
    public MessageDirection Direction { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
}
