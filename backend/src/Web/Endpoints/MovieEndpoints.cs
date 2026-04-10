using backend.Application.Common.Models;
using backend.Application.Movies.Commands.CreateMovie;
using backend.Application.Movies.Commands.DeleteMovie;
using backend.Application.Movies.Commands.UpdateMovie;
using backend.Application.Movies.Queries.GetFilteredMovies;
using backend.Application.Movies.Queries.GetMovieById;
using backend.Application.Movies.Queries.GetMovies;
using backend.Application.Movies.Queries.GetPersonsForMovie;
using backend.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Web.Endpoints;

public class MovieEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/movies";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/all", GetAll).AllowAnonymous();
        group.MapGet("/id/{id:int}", GetById).AllowAnonymous();
        group.MapGet("/filtered", GetFiltered).AllowAnonymous();
        group.MapGet("/id/{id:int}/persons", GetPersonsForMovie).AllowAnonymous();
        group.MapPost("/create", Create).RequireAuthorization();
        group.MapPut("/update", Update).RequireAuthorization();
        group.MapDelete("/delete/id/{id:int}", Delete).RequireAuthorization();
    }

    public static Task<IReadOnlyList<MovieBriefDto>> GetAll(ISender sender)
        => sender.Send(new GetMoviesQuery());

    public static async Task<Results<Ok<MovieBriefDto>, NotFound>> GetById(ISender sender, int id)
    {
        var result = await sender.Send(new GetMovieByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static Task<IReadOnlyList<MovieBriefDto>> GetFiltered(
        ISender sender, string? title, MovieType? type, int? year)
        => sender.Send(new GetFilteredMoviesQuery(title, type, year));

    public static Task<IReadOnlyList<MoviePersonDto>> GetPersonsForMovie(ISender sender, int id)
        => sender.Send(new GetPersonsForMovieQuery(id));

    public static async Task<Results<Ok<Result<int>>, BadRequest<Result<int>>>> Create(
        ISender sender, CreateMovieCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Update(
        ISender sender, UpdateMovieCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Delete(ISender sender, int id)
    {
        var result = await sender.Send(new DeleteMovieCommand(id));
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
