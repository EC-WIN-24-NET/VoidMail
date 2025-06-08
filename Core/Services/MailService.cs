using Azure;
using Azure.Communication.Email;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Core.Services;

public class MailService(IConfiguration configuration) : IMailService
{
    /// <summary>
    /// Got assisted by Google Gemini AI, this method sends an email using Azure Communication Services. and sample code is provided.
    /// Send an email asynchronously using Azure Communication Services.
    /// </summary>
    /// <param name="recipientEmail">The email address of the recipient.</param>
    /// <param name="subject">The subject line of the email.</param>
    /// <param name="htmlBody">The HTML content of the email body.</param>
    /// <param name="plainTextBody">Optional plain text content for the email body. If provided, this will be used for email clients that do not support HTML.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the email was sent successfully; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method relies on configuration values "Communication-Void" (for the email service connection string)
    /// and "EmailSenderAddress" (for the sender's email address) to be present in the application's configuration.
    /// If these configuration values are missing, or if any error occurs during the email sending process,
    /// the method will log an error to the console and return <c>false</c>.
    /// It handles <see cref="Azure.RequestFailedException"/> and general <see cref="System.Exception"/> during the sending process.
    /// </remarks>
    public async Task<bool> SendEmailEventAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        string? plainTextBody
    )
    {
        // Validate the recipient email
        var connectionString = configuration["Communication-Void"];
        var senderAddress = configuration["EmailSenderAddress"];

        // Validate the recipient email
        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(senderAddress))
        {
            // Consider using a proper logging framework here
            Console.WriteLine(
                "Error: Email service configuration (connection string or sender address) is missing."
            );
            return false;
        }

        // Create the email client and email content
        var emailClient = new EmailClient(connectionString);

        // Create the email content
        var emailContent = new EmailContent(subject);
        if (!string.IsNullOrEmpty(plainTextBody))
        {
            emailContent.PlainText = plainTextBody;
        }

        // Set the HTML body
        emailContent.Html = htmlBody;

        // Create the email message with the sender address and recipient email
        var emailMessage = new EmailMessage(
            senderAddress: senderAddress,
            content: emailContent,
            recipients: new EmailRecipients(new List<EmailAddress> { new(recipientEmail) })
        );

        try
        {
            // Send the email asynchronously and wait for the operation to complete
            var emailSendOperation = await emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage,
                CancellationToken.None
            );

            // Check the status of the email send operation
            if (
                emailSendOperation.HasValue
                && emailSendOperation.Value.Status == EmailSendStatus.Succeeded
            )
            {
                Console.WriteLine(
                    $"Email sent successfully. Operation ID: {emailSendOperation.Value.Status}"
                );
                return true;
            }
            else
            {
                // Log detailed error information from emailSendOperation.GetRawResponse() if needed
                var rawResponse = emailSendOperation.GetRawResponse();
                Console.WriteLine(
                    $"Failed to send email. Status: {emailSendOperation.Value?.Status}, Response Status Code: {rawResponse.Status}, Reason: {rawResponse.ReasonPhrase}"
                );
                return false;
            }
        }
        // Catch specific exceptions related to email sending
        catch (RequestFailedException ex)
        {
            Console.WriteLine(
                $"Error sending email (RequestFailedException): {ex.Message}. Status Code: {ex.Status}. ErrorCode: {ex.ErrorCode}"
            );
            // Log ex.ToString() for full details in a real application
            return false;
        }
        // Catch any other exceptions that may occur during the email sending process
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while sending email: {ex.Message}");
            return false;
        }
    }
}
