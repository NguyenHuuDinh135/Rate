using System.Runtime.CompilerServices;
using backend.Application.AI.Interfaces;
using backend.Application.Common.Interfaces;
using backend.Domain.Entities;
using backend.Web.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace backend.Web.Endpoints;

public class AiEndpoints : IEndpointGroup
{
    public static string RoutePrefix => "/api/ai";

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/chat", Chat).AllowAnonymous(); // Should be RequireAuthorization in production
        group.MapPost("/session/create", CreateSession).AllowAnonymous();
        group.MapGet("/sessions", GetSessions).AllowAnonymous();
    }

    public static async IAsyncEnumerable<string> Chat(
        IAIService aiService, 
        ChatSdkRequest request, 
        [EnumeratorCancellation] CancellationToken ct)
    {
        var lastMessage = request.Messages.LastOrDefault(m => m.Role == "user")?.Content ?? "";
        
        await foreach (var token in aiService.ChatAsync(request.SessionId, lastMessage, ct))
        {
            yield return token;
        }
    }

    public static async Task<Ok<AiSessionDto>> CreateSession(
        IApplicationDbContext dbContext,
        CreateSessionRequest request,
        CancellationToken ct)
    {
        var session = new AiSession
        {
            UserId = request.UserId ?? "anonymous",
            Title = request.Title ?? "Hội thoại mới",
            Model = "gpt-4o-mini"
        };

        dbContext.AiSessions.Add(session);
        await dbContext.SaveChangesAsync(ct);

        return TypedResults.Ok(new AiSessionDto(session.Id, session.Title, session.UserId));
    }

    public static async Task<Ok<List<AiSessionDto>>> GetSessions(
        IApplicationDbContext dbContext,
        string userId,
        CancellationToken ct)
    {
        var sessions = await dbContext.AiSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Created)
            .Select(s => new AiSessionDto(s.Id, s.Title, s.UserId))
            .ToListAsync(ct);

        return TypedResults.Ok(sessions);
    }
}

public record ChatSdkRequest(int SessionId, List<ChatMessageDto> Messages);
public record ChatMessageDto(string Role, string Content);
public record CreateSessionRequest(string? UserId, string? Title);
public record AiSessionDto(int Id, string? Title, string UserId);
