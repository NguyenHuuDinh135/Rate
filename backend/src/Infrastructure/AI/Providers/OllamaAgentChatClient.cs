using System.Net.Http.Json;
using System.Text.Json;
using backend.Application.AI.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace backend.Infrastructure.AI.Providers;

public sealed class OllamaAgentChatClient(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<OllamaAgentChatClient> logger) : IAgentChatClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<AgentChatCompletion> CompleteAsync(
        AgentChatRequest request,
        CancellationToken ct = default)
    {
        var endpoint = configuration["Ai:Ollama:Endpoint"] ?? "http://localhost:11434";
        httpClient.BaseAddress ??= new Uri(endpoint.TrimEnd('/') + "/");

        using var payload = BuildPayload(request);
        using var response = await httpClient.PostAsJsonAsync("api/chat", payload.RootElement, JsonOptions, ct);
        var responseText = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Ollama chat failed with status {StatusCode}: {Response}",
                (int)response.StatusCode,
                responseText);

            if (IsNativeToolCallingUnsupported(responseText))
            {
                throw new AgentToolCallingNotSupportedException(
                    $"Ollama model '{request.Model}' does not support native tool calling. " +
                    "Set Ai:Ollama:ToolCallingMode to Prompt for this model.");
            }

            throw new InvalidOperationException(
                $"Ollama chat failed with HTTP {(int)response.StatusCode}: {TrimForError(responseText)}");
        }

        return ParseResponse(responseText);
    }

    private JsonDocument BuildPayload(AgentChatRequest request)
    {
        var contextWindow = Math.Clamp(
            configuration.GetValue<int?>("Ai:Ollama:ContextWindow") ?? 4096,
            512,
            131_072);
        var temperature = Math.Clamp(
            configuration.GetValue<double?>("Ai:Ollama:Temperature") ?? 0.2,
            0,
            2);
        var keepAlive = configuration["Ai:Ollama:KeepAlive"] ?? "10m";

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("model", request.Model);
            writer.WriteBoolean("stream", false);
            writer.WriteString("keep_alive", keepAlive);

            writer.WritePropertyName("messages");
            writer.WriteStartArray();
            foreach (var message in request.Messages)
            {
                writer.WriteStartObject();
                writer.WriteString("role", message.Role);
                writer.WriteString("content", message.Content);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            if (request.ToolChoice != AgentToolChoice.None && request.Tools.Count > 0)
            {
                writer.WritePropertyName("tools");
                writer.WriteStartArray();
                foreach (var tool in request.Tools)
                {
                    writer.WriteStartObject();
                    writer.WriteString("type", "function");
                    writer.WritePropertyName("function");
                    writer.WriteStartObject();
                    writer.WriteString("name", tool.Name);
                    writer.WriteString("description", tool.Description);
                    writer.WritePropertyName("parameters");
                    tool.Parameters.WriteTo(writer);
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();

                if (request.ToolChoice == AgentToolChoice.Required)
                {
                    writer.WriteString("tool_choice", "required");
                }
            }

            writer.WritePropertyName("options");
            writer.WriteStartObject();
            writer.WriteNumber("temperature", temperature);
            writer.WriteNumber("num_ctx", contextWindow);
            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        return JsonDocument.Parse(stream.ToArray());
    }

    private static AgentChatCompletion ParseResponse(string responseText)
    {
        using var document = JsonDocument.Parse(responseText);
        if (!document.RootElement.TryGetProperty("message", out var message))
        {
            return new AgentChatCompletion(string.Empty, []);
        }

        var content = message.TryGetProperty("content", out var contentElement) &&
            contentElement.ValueKind == JsonValueKind.String
                ? contentElement.GetString() ?? string.Empty
                : string.Empty;

        var toolCalls = new List<AgentToolCall>();
        if (message.TryGetProperty("tool_calls", out var toolCallsElement) &&
            toolCallsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var toolCallElement in toolCallsElement.EnumerateArray())
            {
                if (!toolCallElement.TryGetProperty("function", out var function))
                {
                    continue;
                }

                var name = function.TryGetProperty("name", out var nameElement)
                    ? nameElement.GetString()
                    : null;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var arguments = "{}";
                if (function.TryGetProperty("arguments", out var argumentsElement))
                {
                    arguments = argumentsElement.ValueKind == JsonValueKind.String
                        ? NormalizeArguments(argumentsElement.GetString())
                        : argumentsElement.GetRawText();
                }

                var id = toolCallElement.TryGetProperty("id", out var idElement) &&
                    idElement.ValueKind == JsonValueKind.String
                        ? idElement.GetString() ?? Guid.NewGuid().ToString("N")
                        : Guid.NewGuid().ToString("N");

                toolCalls.Add(new AgentToolCall(id, name, arguments));
            }
        }

        return new AgentChatCompletion(content, toolCalls);
    }

    private static string NormalizeArguments(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "{}";
        }

        try
        {
            using var document = JsonDocument.Parse(value);
            return document.RootElement.ValueKind == JsonValueKind.Object
                ? document.RootElement.GetRawText()
                : "{}";
        }
        catch (JsonException)
        {
            return "{}";
        }
    }

    private static string TrimForError(string value)
        => value.Length <= 600 ? value : value[..600];

    private static bool IsNativeToolCallingUnsupported(string responseText)
        => responseText.Contains("does not support tools", StringComparison.OrdinalIgnoreCase);
}
