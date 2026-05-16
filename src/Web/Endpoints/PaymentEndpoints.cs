using backend.Application.Common.Models;
using backend.Application.Payments.Commands.CreatePayment;
using backend.Application.Payments.Commands.DeletePayment;
using backend.Application.Payments.Commands.UpdatePayment;
using backend.Application.Payments.Queries.GetPaymentById;
using backend.Application.Payments.Queries.GetPayments;
using backend.Application.Payments.Queries.GetPaymentsByUser;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Web.Endpoints;

public class PaymentEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/payments";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/all", GetAll).RequireAuthorization();
        group.MapGet("/id/{id:int}", GetById).RequireAuthorization();
        group.MapGet("/users/{userId}", GetByUser).RequireAuthorization();
        group.MapPost("/create", Create).RequireAuthorization();
        group.MapPut("/update", Update).RequireAuthorization();
        group.MapDelete("/delete/{id:int}", Delete).RequireAuthorization();
    }

    public static Task<IReadOnlyList<PaymentBriefDto>> GetAll(ISender sender)
        => sender.Send(new GetPaymentsQuery());

    public static async Task<Results<Ok<PaymentBriefDto>, NotFound>> GetById(ISender sender, int id)
    {
        var result = await sender.Send(new GetPaymentByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static Task<IReadOnlyList<PaymentBriefDto>> GetByUser(ISender sender, string userId)
        => sender.Send(new GetPaymentsByUserQuery(userId));

    public static async Task<Results<Ok<Result<int>>, BadRequest<Result<int>>>> Create(
        ISender sender, CreatePaymentCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Update(
        ISender sender, UpdatePaymentCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Delete(ISender sender, int id)
    {
        var result = await sender.Send(new DeletePaymentCommand(id));
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
