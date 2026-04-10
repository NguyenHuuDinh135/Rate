using backend.Application.Common.Models;
using backend.Application.Movies.Queries.GetMovies;
using backend.Application.Persons.Commands.AddPersonToMovie;
using backend.Application.Persons.Commands.CreatePerson;
using backend.Application.Persons.Commands.DeletePerson;
using backend.Application.Persons.Commands.RemovePersonFromMovie;
using backend.Application.Persons.Commands.UpdatePerson;
using backend.Application.Persons.Queries.GetMoviesByPerson;
using backend.Application.Persons.Queries.GetPersonById;
using backend.Application.Persons.Queries.GetPersons;
using backend.Application.Persons.Queries.GetPersonsByMovie;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Web.Endpoints;

public class PersonEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/persons";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/all", GetAll).AllowAnonymous();
        group.MapGet("/id/{id:int}", GetById).AllowAnonymous();
        group.MapGet("/movies/{movieId:int}", GetByMovie).AllowAnonymous();
        group.MapGet("/id/{id:int}/movies", GetMoviesByPerson).AllowAnonymous();
        group.MapPost("/create", Create).RequireAuthorization();
        group.MapPut("/update", Update).RequireAuthorization();
        group.MapDelete("/delete/{id:int}", Delete).RequireAuthorization();
        group.MapPost("/movies/add", AddToMovie).RequireAuthorization();
        group.MapDelete("/movies/remove", RemoveFromMovie).RequireAuthorization();
    }

    public static Task<IReadOnlyList<PersonBriefDto>> GetAll(ISender sender)
        => sender.Send(new GetPersonsQuery());

    public static async Task<Results<Ok<PersonBriefDto>, NotFound>> GetById(ISender sender, int id)
    {
        var result = await sender.Send(new GetPersonByIdQuery(id));
        return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    public static Task<IReadOnlyList<PersonBriefDto>> GetByMovie(ISender sender, int movieId)
        => sender.Send(new GetPersonsByMovieQuery(movieId));

    public static Task<IReadOnlyList<MovieBriefDto>> GetMoviesByPerson(ISender sender, int id)
        => sender.Send(new GetMoviesByPersonQuery(id));

    public static async Task<Results<Ok<Result<int>>, BadRequest<Result<int>>>> Create(
        ISender sender, CreatePersonCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Update(
        ISender sender, UpdatePersonCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> Delete(ISender sender, int id)
    {
        var result = await sender.Send(new DeletePersonCommand(id));
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<Ok, BadRequest<Result>>> AddToMovie(
        ISender sender, AddPersonToMovieCommand request)
    {
        var result = await sender.Send(request);
        return result.Succeeded ? TypedResults.Ok() : TypedResults.BadRequest(result);
    }

    public static async Task<Results<NoContent, BadRequest<Result>>> RemoveFromMovie(
        ISender sender, int movieId, int personId)
    {
        var result = await sender.Send(new RemovePersonFromMovieCommand(movieId, personId));
        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result);
    }
}
