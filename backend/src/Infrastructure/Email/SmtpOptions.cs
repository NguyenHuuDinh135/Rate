using System.ComponentModel.DataAnnotations;

namespace backend.Infrastructure.Email;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    [Required]
    public string Host { get; init; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; init; } = 587;

    [Required]
    public string FromEmail { get; init; } = string.Empty;

    public string FromName { get; init; } = "Rate";

    public string? UserName { get; init; }
    public string? Password { get; init; }

    public bool EnableSsl { get; init; } = true;
}

