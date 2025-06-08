using System.ComponentModel.DataAnnotations;

namespace VoidMail.DTos;

/// <summary>
/// Represents the data transfer object for an email message.
/// This class includes properties for the recipient's email address,
/// the email subject, the HTML body, and an optional plain text body.
/// It also includes validation attributes to ensure the data integrity.
/// </summary>
/// <example>Hello! This is a test email.</example>
public class EmailDto
{
    /// <summary>
    /// The recipient's email address.
    /// </summary>
    /// <example>recipient@example.com</example>
    [Required(ErrorMessage = "Recipient email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// The subject of the email.
    /// </summary>
    /// <example>Meeting Reminder</example>
    [Required(ErrorMessage = "Email subject is required.")]
    [StringLength(
        255,
        MinimumLength = 1,
        ErrorMessage = "Subject must be between 1 and 255 characters."
    )]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// The HTML version of the email body.
    /// </summary>
    /// <example>&lt;html&gt;&lt;body&gt;&lt;h1&gt;Hello!&lt;/h1&gt;&lt;p&gt;This is a test email.&lt;/p&gt;&lt;/body&gt;&lt;/html&gt;</example>
    [Required(ErrorMessage = "HTML body is required.")]
    public string HtmlBody { get; set; } = string.Empty;

    /// <summary>
    /// The plain text version of the email body. Optional.
    /// If not provided, some email clients might attempt to generate it from the HTML body.
    /// </summary>
    /// <example>Hello! This is a test email.</example>
    public string? PlainTextBody { get; set; }
}
