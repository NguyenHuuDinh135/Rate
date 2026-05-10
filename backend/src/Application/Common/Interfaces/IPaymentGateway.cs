namespace backend.Application.Common.Interfaces;

public interface IPaymentGateway
{
    Task<PaymentGatewayResult> CreateCheckoutAsync(
        string orderId,
        decimal amount,
        string currency,
        string description,
        CancellationToken cancellationToken = default);

    bool VerifyWebhookSignature(string payload, string signatureHeader);
}

public sealed class PaymentGatewayResult
{
    public string ExternalPaymentId { get; init; } = string.Empty;
    public string CheckoutUrl { get; init; } = string.Empty;
}

