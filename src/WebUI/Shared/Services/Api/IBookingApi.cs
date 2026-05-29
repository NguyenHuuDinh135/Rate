using Refit;
using WebUI.Shared.Models.Bookings;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IBookingApi
{
    [Get("/api/bookings/all")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<BookingDto>>> GetAllAsync();

    [Get("/api/bookings/id/{id}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<BookingDto>> GetByIdAsync(int id);

    [Get("/api/bookings/users/{userId}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<BookingDto>>> GetByUserAsync(string userId);

    [Get("/api/bookings/shows/{showId}")]
    Task<WebUI.Shared.Models.Common.ApiResponse<List<BookedSeatDto>>> GetBookedSeatsAsync(int showId);

    [Post("/api/bookings/create")]
    Task<OperationResultDto<int>> CreateAsync([Body] CreateBookingCommand payload);

    [Put("/api/bookings/update")]
    Task UpdateAsync([Body] UpdateBookingCommand payload);

    [Delete("/api/bookings/delete/{id}")]
    Task DeleteAsync(int id);
}
