using System.ComponentModel.DataAnnotations;

namespace backend.Infrastructure.Payments;

public sealed class PaymentGatewayOptions
{
    public const string SectionName = "PaymentGateway";

    [Required]
    public string BaseUrl { get; init; } = string.Empty;

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string WebhookSecret { get; init; } = string.Empty;
}

