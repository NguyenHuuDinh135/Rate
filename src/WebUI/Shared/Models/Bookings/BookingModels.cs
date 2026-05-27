using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Models.Bookings;

public class BookingDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int? ShowId { get; set; }
    public string? SeatRow { get; set; }
    public int? SeatNumber { get; set; }
    public decimal Price { get; set; }
    public BookingStatus Status { get; set; }
    public string? BookingDatetime { get; set; }
}

public class BookedSeatDto
{
    public string SeatRow { get; set; } = "";
    public int SeatNumber { get; set; }
    public bool? IsBooked { get; set; }
}

public record CreateBookingCommand(
    int ShowId,
    string SeatRow,
    int SeatNumber,
    float Price,
    string? UserId = null);

public record UpdateBookingCommand(
    int Id,
    string SeatRow,
    int SeatNumber,
    float Price,
    int Status);
