using System.Text.Json;

namespace backend.Application.AI.Interfaces;

public interface IAgentChatClient
{
    Task<AgentChatCompletion> CompleteAsync(AgentChatRequest request, CancellationToken ct = default);
}

public sealed class AgentToolCallingNotSupportedException(string message) : Exception(message);

public sealed record AgentChatRequest(
    string Model,
    IReadOnlyList<AgentChatMessage> Messages,
    IReadOnlyList<AgentToolDefinition> Tools,
    AgentToolChoice ToolChoice);

public sealed record AgentChatMessage(string Role, string Content)
{
    public static AgentChatMessage System(string content) => new("system", content);
    public static AgentChatMessage User(string content) => new("user", content);
    public static AgentChatMessage Assistant(string content) => new("assistant", content);
    public static AgentChatMessage Tool(string content) => new("tool", content);
}

public sealed record AgentToolDefinition(
    string Name,
    string Description,
    JsonElement Parameters);

public enum AgentToolChoice
{
    None,
    Auto,
    Required
}

public sealed record AgentChatCompletion(
    string Content,
    IReadOnlyList<AgentToolCall> ToolCalls);

public sealed record AgentToolCall(
    string Id,
    string Name,
    string Arguments);
