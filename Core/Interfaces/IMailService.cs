namespace Core.Interfaces;

public interface IMailService
{
    Task<bool> SendEmailEventAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        string? plainTextBody
    );
}
