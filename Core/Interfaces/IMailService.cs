

namespace Core.Interfaces;

public interface IEventService
{
Task<bool> SendEmailEventAsync(string recipientEmail, string subject, string htmlBody, string? plainTextBody);
}
