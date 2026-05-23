using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using backend.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace backend.Infrastructure.Payments;

public sealed class HttpPaymentGateway(
    HttpClient httpClient,
    IOptions<PaymentGatewayOptions> options) : IPaymentGateway
{
    private readonly PaymentGatewayOptions _options = options.Value;

    public async Task<PaymentGatewayResult> CreateCheckoutAsync(
        string orderId,
        decimal amount,
        string currency,
        string description,
        CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(new
        {
            orderId,
            amount,
            currency,
            description
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, "/checkout")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Payment gateway create checkout failed: {(int)response.StatusCode}");
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var parsed = JsonSerializer.Deserialize<CheckoutResponse>(body) ?? new CheckoutResponse();
        return new PaymentGatewayResult
        {
            ExternalPaymentId = parsed.PaymentId ?? orderId,
            CheckoutUrl = parsed.CheckoutUrl ?? string.Empty
        };
    }

    public bool VerifyWebhookSignature(string payload, string signatureHeader)
    {
        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            return false;
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.WebhookSecret));
        var computed = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
        return computed == signatureHeader.Trim().ToLowerInvariant();
    }

    private sealed class CheckoutResponse
    {
        public string? PaymentId { get; init; }
        public string? CheckoutUrl { get; init; }
    }
}

