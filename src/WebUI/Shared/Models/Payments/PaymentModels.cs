using System.Text.Json.Serialization;
using WebUI.Shared.Models.Common;
using WebUI.Shared.Models.Movies;

namespace WebUI.Shared.Models.Payments;

public record CreatePaymentCommand(
    int Amount,
    int Method,
    string UserId,
    int ShowId);

public record UpdatePaymentCommand(int Id, decimal Amount, int PaymentMethod);

public class PaymentDto
{
    [JsonPropertyName("id")]
    public int PaymentId { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("paymentDateTime")]
    public string PaymentDatetime { get; set; } = "";

    [JsonPropertyName("method")]
    public int PaymentMethod { get; set; }

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

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("genres")]
    public List<GenreDto>? Genres { get; set; }
}

