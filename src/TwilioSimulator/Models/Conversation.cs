namespace TwilioSimulator.Models;

public class Conversation
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public List<SmsMessage> Messages { get; set; } = new();
}
