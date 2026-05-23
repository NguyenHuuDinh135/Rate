using Refit;
using WebUI.Shared.Models.Bookings;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IBookingApi
{
    [Get("/api/bookings/all")]
    Task<List<BookingDto>> GetAllAsync();

    [Get("/api/bookings/id/{id}")]
    Task<BookingDto> GetByIdAsync(int id);

    [Get("/api/bookings/users/{userId}")]
    Task<List<BookingDto>> GetByUserAsync(string userId);

    [Get("/api/bookings/shows/{showId}")]
    Task<List<BookedSeatDto>> GetBookedSeatsAsync(int showId);

    [Post("/api/bookings/create")]
    Task<int> CreateAsync([Body] CreateBookingCommand payload);

    [Put("/api/bookings/update")]
    Task UpdateAsync([Body] UpdateBookingCommand payload);

    [Delete("/api/bookings/delete/{id}")]
    Task DeleteAsync(int id);
}
