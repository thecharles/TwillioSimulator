using System.Text.Json.Serialization;

namespace TwilioSimulator.DTOs;

public class SmsSendDto
{
    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public string To { get; set; } = string.Empty;

    [JsonPropertyName("messageBody")]
    public string MessageBody { get; set; } = string.Empty;
}
