using System.Text.Json.Serialization;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Models.Payments;

public record CreatePaymentCommand(
    decimal Amount,
    string PaymentDatetime,
    string PaymentMethod,
    string UserId,
    int ShowId,
    List<int> Bookings);

public record UpdatePaymentCommand(int Id, decimal Amount, PaymentMethod PaymentMethod);

public class PaymentDto
{
    [JsonPropertyName("id")]
    public int PaymentId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("paymentDateTime")]
    public string PaymentDatetime { get; set; } = "";

    [JsonPropertyName("method")]
    public string PaymentMethod { get; set; } = "";

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("showId")]
    public int? ShowId { get; set; }

    [JsonPropertyName("movie")]
    public PaymentMovieDto? Movie { get; set; }
}

public class PaymentMovieDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("posterUrl")]
    public string PosterUrl { get; set; } = "";
}

