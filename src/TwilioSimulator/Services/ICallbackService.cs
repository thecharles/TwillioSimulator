using TwilioSimulator.DTOs;

namespace TwilioSimulator.Services;

public interface ICallbackService
{
    Task<bool> SendCallbackAsync(string callbackUrl, SmsSendDto payload);
}
