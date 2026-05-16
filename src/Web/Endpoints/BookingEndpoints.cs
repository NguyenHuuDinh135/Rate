using backend.Application.Bookings.Commands.CreateBooking;
using backend.Application.Bookings.Commands.DeleteBooking;
using backend.Application.Bookings.Commands.UpdateBooking;
using backend.Application.Bookings.Queries.GetBookingById;
using backend.Application.Bookings.Queries.GetBookings;
using backend.Application.Bookings.Queries.GetBookingsByShow;
using backend.Application.Bookings.Queries.GetBookingsByUser;
using backend.Application.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Web.Endpoints;

public class BookingEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/bookings";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/all", GetAll).RequireAuthorization();
        group.MapGet("/id/{id:int}", GetById).RequireAuthorization();
        group.MapGet("/users/{userId}", GetByUser).RequireAuthorization();
        group.MapGet("/shows/{showId:int}", GetByShow).RequireAuthorization();
        group.MapPost("/create", Create).RequireAuthorization();
        group.MapPut("/update", Update).RequireAuthorization();
        group.MapDelete("/delete/{id:int}", Delete).RequireAuthorization();
    }

    public static Task<IReadOnlyList<BookingBriefDto>> GetAll(ISender sender)
        => sender.Send(new GetBookingsQuery());

    public static async Task<Results<Ok<BookingBriefDto>, NotFound>> GetById(ISender sender, int id)
    {
        var result = await sender.Send(new GetBookingByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static Task<IReadOnlyList<BookingBriefDto>> GetByUser(ISender sender, string userId)
        => sender.Send(new GetBookingsByUserQuery(userId));

    public static Task<IReadOnlyList<BookingBriefDto>> GetByShow(ISender sender, int showId)
        => sender.Send(new GetBookingsByShowQuery(showId));

    public static async Task<Results<Ok<Result<int>>, BadRequest<Result<int>>>> Create(
        ISender sender, CreateBookingCommand request, HttpContext httpContext)
    {
        var idemKey = httpContext.Request.Headers["Idempotency-Key"].ToString();
        var result = await sender.Send(request with { IdempotencyKey = idemKey });
        return result.Succeeded ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Update(
        ISender sender, UpdateBookingCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Delete(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteBookingCommand(id));
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
