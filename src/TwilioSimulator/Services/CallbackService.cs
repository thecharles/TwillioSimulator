using System.Text;
using System.Text.Json;
using TwilioSimulator.DTOs;

namespace TwilioSimulator.Services;

public class CallbackService : ICallbackService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CallbackService> _logger;

    public CallbackService(IHttpClientFactory httpClientFactory, ILogger<CallbackService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> SendCallbackAsync(string callbackUrl, SmsSendDto payload)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending callback to {Url} with payload: {Payload}", callbackUrl, json);

            var response = await client.PostAsync(callbackUrl, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Callback sent successfully to {Url}", callbackUrl);
                return true;
            }

            _logger.LogWarning("Callback failed with status {StatusCode} to {Url}", response.StatusCode, callbackUrl);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending callback to {Url}", callbackUrl);
            return false;
        }
    }
}
