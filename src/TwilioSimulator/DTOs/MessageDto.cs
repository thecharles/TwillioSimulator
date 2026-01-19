namespace TwilioSimulator.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string MessageBody { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
