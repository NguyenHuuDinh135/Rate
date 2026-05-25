using Refit;
using WebUI.Shared.Models.Payments;
using WebUI.Shared.Models.Common;

namespace WebUI.Shared.Services.Api;

public interface IPaymentApi
{
    [Get("/api/payments/all")]
    Task<List<PaymentDto>> GetAllAsync();

    [Get("/api/payments/id/{id}")]
    Task<PaymentDto> GetByIdAsync(int id);

    [Get("/api/payments/users/{userId}")]
    Task<List<PaymentDto>> GetByUserAsync(string userId);

    [Post("/api/payments/create")]
    Task<OperationResultDto<int>> CreateAsync([Body] CreatePaymentCommand payload);

    [Put("/api/payments/update")]
    Task UpdateAsync([Body] UpdatePaymentCommand payload);

    [Delete("/api/payments/delete/{id}")]
    Task DeleteAsync(int id);
}
