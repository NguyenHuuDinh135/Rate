using System.Net;
using System.Text;
using backend.Application.AI.Interfaces;
using backend.Infrastructure.AI.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Shouldly;

namespace backend.Application.UnitTests.AI;

public class OllamaAgentChatClientTests
{
    [Test]
    public async Task CompleteAsync_ShouldAllowMultipleRequests_WithSameHttpClient()
    {
        var handler = new StubHttpMessageHandler(
            """
            {"message":{"role":"assistant","content":"OK 1"}}
            """,
            """
            {"message":{"role":"assistant","content":"OK 2"}}
            """);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/"),
            Timeout = TimeSpan.FromSeconds(5)
        };

        var client = CreateClient(httpClient);

        var first = await client.CompleteAsync(CreateRequest());
        var second = await client.CompleteAsync(CreateRequest());

        first.Content.ShouldBe("OK 1");
        second.Content.ShouldBe("OK 2");
        handler.RequestCount.ShouldBe(2);
    }

    [Test]
    public async Task CompleteAsync_ShouldThrowSpecificException_WhenModelDoesNotSupportTools()
    {
        var handler = new StubHttpMessageHandler(
            """{"error":"registry.ollama.ai/library/qwen2.5vl:7b does not support tools"}""",
            HttpStatusCode.InternalServerError);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:11434/"),
            Timeout = TimeSpan.FromSeconds(5)
        };

        var client = CreateClient(httpClient);

        var exception = await Should.ThrowAsync<AgentToolCallingNotSupportedException>(
            () => client.CompleteAsync(CreateRequest()));

        exception.Message.ShouldContain("does not support native tool calling");
    }

    private static OllamaAgentChatClient CreateClient(HttpClient httpClient)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ai:Ollama:Endpoint"] = "http://localhost:11434",
                ["Ai:Ollama:ContextWindow"] = "2048",
                ["Ai:Ollama:Temperature"] = "0.2",
                ["Ai:Ollama:KeepAlive"] = "1m"
            })
            .Build();

        return new OllamaAgentChatClient(
            httpClient,
            configuration,
            NullLogger<OllamaAgentChatClient>.Instance);
    }

    private static AgentChatRequest CreateRequest()
        => new(
            "qwen2.5:7b",
            [AgentChatMessage.User("Xin chào")],
            [],
            AgentToolChoice.None);

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> responses = new();

        public StubHttpMessageHandler(params string[] responseBodies)
        {
            foreach (var body in responseBodies)
            {
                responses.Enqueue(CreateResponse(body, HttpStatusCode.OK));
            }
        }

        public StubHttpMessageHandler(string responseBody, HttpStatusCode statusCode)
        {
            responses.Enqueue(CreateResponse(responseBody, statusCode));
        }

        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestCount++;
            if (responses.Count == 0)
            {
                return Task.FromResult(CreateResponse(
                    """{"message":{"role":"assistant","content":"OK"}}""",
                    HttpStatusCode.OK));
            }

            return Task.FromResult(responses.Dequeue());
        }

        private static HttpResponseMessage CreateResponse(string body, HttpStatusCode statusCode)
            => new(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
    }
}
