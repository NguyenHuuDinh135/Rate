using Refit;
using WebFrontend.Shared.Models.Payments;
using WebFrontend.Shared.Models.Common;

namespace WebFrontend.Shared.Services.Api;

public interface IPaymentApi
{
    [Get("/api/payments/all")]
    Task<WebFrontend.Shared.Models.Common.ApiResponse<List<PaymentDto>>> GetAllAsync();

    [Get("/api/payments/id/{id}")]
    Task<WebFrontend.Shared.Models.Common.ApiResponse<PaymentDto>> GetByIdAsync(int id);

    [Get("/api/payments/users/{userId}")]
    Task<WebFrontend.Shared.Models.Common.ApiResponse<List<PaymentDto>>> GetByUserAsync(string userId);

    [Post("/api/payments/create")]
    Task<int> CreateAsync([Body] CreatePaymentCommand payload);

    [Put("/api/payments/update")]
    Task UpdateAsync([Body] UpdatePaymentCommand payload);

    [Delete("/api/payments/delete/{id}")]
    Task DeleteAsync(int id);
}
