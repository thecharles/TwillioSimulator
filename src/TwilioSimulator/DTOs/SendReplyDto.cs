using System.Text.Json.Serialization;

namespace TwilioSimulator.DTOs;

public class SendReplyDto
{
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("messageBody")]
    public string MessageBody { get; set; } = string.Empty;
}
