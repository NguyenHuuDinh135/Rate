using WebFrontend.Shared.Models.Common;

namespace WebFrontend.Shared.Models.Payments;

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
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentDatetime { get; set; } = "";
    public string PaymentMethod { get; set; } = "";
    public string? UserId { get; set; }
    public int? ShowId { get; set; }
    public PaymentMovieDto? Movie { get; set; }
}

public class PaymentMovieDto
{
    public string Title { get; set; } = "";
    public string PosterUrl { get; set; } = "";
}
