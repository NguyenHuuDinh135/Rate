using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Application.FunctionalTests.Infrastructure;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.FunctionalTests.AI;

public class AiEndpointsTests : TestBase
{
    [Test]
    public async Task ChatSync_ShouldReturnJsonResponse()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);
        await SeedActionMovieAsync("MAD MAX FURY ROAD");

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "Gợi ý cho tôi một số bộ phim hành động trong hệ thống"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        root.GetProperty("sessionId").GetInt32().ShouldBe(sessionId);
        root.GetProperty("model").GetString().ShouldBe("qwen2.5:7b");
        root.GetProperty("inputMessage").GetString()
            .ShouldBe("Gợi ý cho tôi một số bộ phim hành động trong hệ thống");
        root.GetProperty("inputSource").GetString().ShouldBe("message");
        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("Mình gợi ý");
        message.ShouldContain("MAD MAX FURY ROAD");
        message.ShouldNotContain("LLM response from test chat service");
        root.GetProperty("toolCalls").GetArrayLength().ShouldBe(1);
        root.GetProperty("toolCalls")[0].GetProperty("name").GetString().ShouldBe("search_movies");
        var toolResult = root.GetProperty("toolCalls")[0].GetProperty("result").GetString();
        toolResult.ShouldNotBeNull();
        toolResult.ShouldContain("MAD MAX FURY ROAD");
    }

    [Test]
    public async Task Chat_ShouldAcceptAiSdkMessagesPayload()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);

        var response = await client.PostAsJsonAsync("/api/ai/chat", new
        {
            sessionId,
            messages = new[]
            {
                new { role = "user", content = "Xin chào" }
            }
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;
        root.GetProperty("inputMessage").GetString().ShouldBe("Xin chào");
        root.GetProperty("inputSource").GetString().ShouldBe("messages");
        root.GetProperty("message").GetString()
            .ShouldBe("LLM response from test chat service");
    }

    [Test]
    public async Task ChatSync_ShouldPreferTopLevelMessage_WhenMessagesContainDifferentUserText()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);
        await SeedMovieWithGenreAsync("BATMAN BEGINS", "Action");

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "Tôi cần tìm thông tin bộ phim Batman",
            messages = new[]
            {
                new { role = "user", content = "Hello" }
            }
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        root.GetProperty("model").GetString().ShouldBe("qwen2.5:7b");
        root.GetProperty("inputMessage").GetString().ShouldBe("Tôi cần tìm thông tin bộ phim Batman");
        root.GetProperty("inputSource").GetString().ShouldBe("message");
        root.GetProperty("toolCalls").GetArrayLength().ShouldBe(1);
        root.GetProperty("toolCalls")[0].GetProperty("name").GetString().ShouldBe("find_movie_details");

        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("BATMAN BEGINS");
        message.ShouldContain("Action");
        message.ShouldNotContain("Mô hình AI local đang gặp lỗi");
        message.ShouldNotContain("Hello");

        var arguments = root.GetProperty("toolCalls")[0].GetProperty("arguments").GetString();
        arguments.ShouldNotBeNull();
        using var argumentsDocument = JsonDocument.Parse(arguments);
        argumentsDocument.RootElement.GetProperty("query").GetString()
            .ShouldBe("Tôi cần tìm thông tin bộ phim Batman");
    }

    [Test]
    public async Task ChatSync_ShouldUseTopLevelMessage_WhenMessagesContentIsEmpty()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);
        await SeedActionMovieAsync("EDGE OF TOMORROW");

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "Gợi ý cho tôi một số bộ phim hành động trong hệ thống",
            messages = new[]
            {
                new { role = "user", content = "" }
            }
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("Bạn có thể thử");
        message.ShouldContain("EDGE OF TOMORROW");

        var arguments = root.GetProperty("toolCalls")[0].GetProperty("arguments").GetString();
        arguments.ShouldNotBeNull();
        using var argumentsDocument = JsonDocument.Parse(arguments);
        argumentsDocument.RootElement.GetProperty("query").GetString()
            .ShouldBe("Gợi ý cho tôi một số bộ phim hành động trong hệ thống");
    }

    [Test]
    public async Task ChatSync_ShouldUseListGenresTool()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);
        await SeedGenresAsync("Action", "Romance");

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "Hệ thống có những thể loại phim nào?"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        root.GetProperty("toolCalls")[0].GetProperty("name").GetString().ShouldBe("list_genres");
        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("Action");
        message.ShouldContain("Romance");
    }

    [Test]
    public async Task ChatSync_ShouldUseEmbeddingStatusTool()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);
        await SeedMovieWithoutEmbeddingAsync("EMBEDDING STATUS MOVIE");

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "Trạng thái embedding/vector trong database hiện tại thế nào?"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        root.GetProperty("toolCalls")[0].GetProperty("name").GetString().ShouldBe("get_embedding_status");
        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("embedding");
        message.ShouldContain("vector 1024 chiều");

        var toolResult = root.GetProperty("toolCalls")[0].GetProperty("result").GetString();
        toolResult.ShouldNotBeNull();
        toolResult.ShouldContain("\"type\":\"embedding_status\"");
        toolResult.ShouldContain("\"pending\"");
    }

    [Test]
    public async Task ChatSync_ShouldUseModelPlanner_WhenNativeToolCallIsMissing()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);
        await SeedActionMovieAsync("PLANNER ACTION MOVIE");

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "NO_NATIVE_TOOL Gợi ý cho tôi phim hành động trong hệ thống"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        root.GetProperty("toolCalls").GetArrayLength().ShouldBe(1);
        root.GetProperty("toolCalls")[0].GetProperty("name").GetString().ShouldBe("search_movies");
        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("PLANNER ACTION MOVIE");
    }

    [Test]
    public async Task ChatSync_ShouldFallbackToPlanner_WhenNativeToolCallRequestFails()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);
        await SeedActionMovieAsync("NATIVE FALLBACK ACTION MOVIE");

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "FORCE_NATIVE_ERROR Gợi ý cho tôi phim hành động trong hệ thống"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("NATIVE FALLBACK ACTION MOVIE");
        message.ShouldNotContain("Mô hình AI local đang gặp lỗi");
        root.GetProperty("toolCalls").GetArrayLength().ShouldBe(1);
        root.GetProperty("toolCalls")[0].GetProperty("name").GetString().ShouldBe("search_movies");
    }

    [Test]
    public async Task ChatSync_ShouldReturnGroundedMovieDetails_WhenLocalLlmFails()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);
        await SeedMovieWithGenreAsync("DUNE", "Sci-Fi");

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "FORCE_LLM_ERROR Phim DUNE là thể loại phim gì?",
            messages = new[]
            {
                new { role = "user", content = "" }
            }
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("DUNE");
        message.ShouldContain("Sci-Fi");
        message.ShouldNotContain("LLM response from test chat service");

        root.GetProperty("toolCalls").GetArrayLength().ShouldBe(1);
        root.GetProperty("toolCalls")[0].GetProperty("name").GetString().ShouldBe("find_movie_details");
        var toolResult = root.GetProperty("toolCalls")[0].GetProperty("result").GetString();
        toolResult.ShouldNotBeNull();
        toolResult.ShouldContain("\"type\":\"movie_details_search\"");
        toolResult.ShouldContain("DUNE");
        toolResult.ShouldContain("Sci-Fi");
    }

    [Test]
    public async Task Chat_ShouldReturnBadRequest_WhenPayloadIsInvalid()
    {
        using var client = TestApp.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/chat", new
        {
            message = ""
        });

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        document.RootElement.GetProperty("error").GetString()
            .ShouldBe("Thiếu sessionId hoặc nội dung tin nhắn.");
    }

    [Test]
    public async Task Chat_ShouldReturnNotFound_WhenSessionDoesNotExist()
    {
        using var client = TestApp.CreateClient();

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId = -1,
            message = "Xin chào"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ChatSync_ShouldReturnStructuredToolError_WhenToolCannotRun()
    {
        using var client = TestApp.CreateClient();
        var sessionId = await CreateSessionAsync(client);

        var response = await client.PostAsJsonAsync("/api/ai/chat/sync", new
        {
            sessionId,
            message = "FORCE_UNKNOWN_TOOL kiểm tra lỗi tool"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        var message = root.GetProperty("message").GetString();
        message.ShouldNotBeNull();
        message.ShouldContain("gặp sự cố");
        message.ShouldNotContain("Mô hình AI local đang gặp lỗi");

        root.GetProperty("toolCalls").GetArrayLength().ShouldBe(1);
        var toolCall = root.GetProperty("toolCalls")[0];
        toolCall.GetProperty("name").GetString().ShouldBe("unknown_tool");
        var toolResult = toolCall.GetProperty("result").GetString();
        toolResult.ShouldNotBeNull();
        toolResult.ShouldContain("\"type\":\"tool_error\"");
        toolResult.ShouldContain("\"code\":\"unknown_tool\"");
    }

    [Test]
    public async Task ModelHealth_ShouldReportNativeToolMode_ForAgentModel()
    {
        using var client = TestApp.CreateClient();

        var response = await client.GetAsync("/api/ai/model/health");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        root.GetProperty("model").GetString().ShouldBe("qwen2.5:7b");
        root.GetProperty("endpoint").GetString().ShouldBe("http://localhost:11434");
        root.GetProperty("toolCallingMode").GetString().ShouldBe("Auto");
        root.GetProperty("chatAvailable").GetBoolean().ShouldBeTrue();
        root.GetProperty("nativeToolCallingSupported").GetBoolean().ShouldBeTrue();
        root.GetProperty("nativeToolCallingError").ValueKind.ShouldBe(JsonValueKind.Null);
        root.GetProperty("recommendedToolCallingMode").GetString().ShouldBe("Auto");
    }

    [Test]
    public async Task EmbeddingsStatus_ShouldReportPendingMovies()
    {
        using var client = TestApp.CreateClient();
        await SeedMovieWithoutEmbeddingAsync("PENDING EMBEDDING MOVIE");

        var response = await client.GetAsync("/api/ai/embeddings/status");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        root.GetProperty("model").GetString().ShouldBe("bge-m3:latest");
        root.GetProperty("dimensions").GetInt32().ShouldBe(1024);
        root.GetProperty("movies").GetProperty("pending").GetInt32().ShouldBeGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task EmbeddingsSync_ShouldPopulateMovieEmbeddings()
    {
        using var client = TestApp.CreateClient();
        var movieId = await SeedMovieWithoutEmbeddingAsync("SYNC EMBEDDING MOVIE");

        var response = await client.PostAsJsonAsync("/api/ai/embeddings/sync", new
        {
            target = "movies",
            batchSize = 10
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var result = document.RootElement.GetProperty("results")[0];
        result.GetProperty("target").GetString().ShouldBe("movies");
        result.GetProperty("attempted").GetInt32().ShouldBeGreaterThanOrEqualTo(1);
        result.GetProperty("succeeded").GetInt32().ShouldBeGreaterThanOrEqualTo(1);
        result.GetProperty("failed").GetInt32().ShouldBe(0);

        await TestApp.ExecuteDbContextAsync(async db =>
        {
            var movie = await db.Movies.FindAsync(movieId);
            movie.ShouldNotBeNull();
            movie.Embedding.ShouldNotBeNull();
        });
    }

    [Test]
    public async Task Prompts_ShouldCreateVersionedPromptAndActivateSelectedVersion()
    {
        using var client = TestApp.CreateClient();

        var firstResponse = await client.PostAsJsonAsync("/api/ai/prompts", new
        {
            name = "DefaultSystemPrompt",
            template = "Prompt version 1",
            description = "Initial prompt"
        });

        firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var firstDocument = JsonDocument.Parse(await firstResponse.Content.ReadAsStringAsync());
        var firstId = firstDocument.RootElement.GetProperty("id").GetInt32();
        firstDocument.RootElement.GetProperty("version").GetInt32().ShouldBe(1);
        firstDocument.RootElement.GetProperty("isActive").GetBoolean().ShouldBeTrue();

        var secondResponse = await client.PostAsJsonAsync("/api/ai/prompts", new
        {
            name = "DefaultSystemPrompt",
            template = "Prompt version 2",
            description = "Updated prompt"
        });

        secondResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var secondDocument = JsonDocument.Parse(await secondResponse.Content.ReadAsStringAsync());
        secondDocument.RootElement.GetProperty("version").GetInt32().ShouldBe(2);
        secondDocument.RootElement.GetProperty("isActive").GetBoolean().ShouldBeTrue();

        var activeResponse = await client.GetAsync("/api/ai/prompts/DefaultSystemPrompt");
        activeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var activeDocument = JsonDocument.Parse(await activeResponse.Content.ReadAsStringAsync());
        activeDocument.RootElement.GetProperty("template").GetString().ShouldBe("Prompt version 2");

        var activateFirstResponse = await client.PostAsync($"/api/ai/prompts/{firstId}/activate", null);
        activateFirstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        activeResponse = await client.GetAsync("/api/ai/prompts/DefaultSystemPrompt");
        activeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var reactivatedDocument = JsonDocument.Parse(await activeResponse.Content.ReadAsStringAsync());
        reactivatedDocument.RootElement.GetProperty("template").GetString().ShouldBe("Prompt version 1");

        var listResponse = await client.GetAsync("/api/ai/prompts");
        listResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var listDocument = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        listDocument.RootElement.GetArrayLength().ShouldBe(2);
    }

    private static async Task<int> CreateSessionAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/ai/session/create", new
        {
            userId = "test-user",
            title = "Test session"
        });

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("id").GetInt32();
    }

    private static Task SeedActionMovieAsync(string title)
    {
        return TestApp.ExecuteDbContextAsync(async db =>
        {
            var action = new Genre { Name = "Action" };
            var movie = new Movie
            {
                Title = title,
                Summary = "A high energy action movie with intense battles.",
                Year = 2024,
                Rating = 8.2m,
                MovieType = MovieType.NowShowing
            };

            db.Genres.Add(action);
            db.Movies.Add(movie);
            db.MovieGenres.Add(new MovieGenre { Movie = movie, Genre = action });
            await db.SaveChangesAsync();
        });
    }

    private static Task SeedMovieWithGenreAsync(string title, string genreName)
    {
        return TestApp.ExecuteDbContextAsync(async db =>
        {
            var genre = new Genre { Name = genreName };
            var movie = new Movie
            {
                Title = title,
                Summary = "A desert science fiction epic with political conflict and survival.",
                Year = 2021,
                Rating = 8.0m,
                MovieType = MovieType.NowShowing
            };

            db.Genres.Add(genre);
            db.Movies.Add(movie);
            db.MovieGenres.Add(new MovieGenre { Movie = movie, Genre = genre });
            await db.SaveChangesAsync();
        });
    }

    private static async Task<int> SeedMovieWithoutEmbeddingAsync(string title)
    {
        var movieId = 0;
        await TestApp.ExecuteDbContextAsync(async db =>
        {
            var movie = new Movie
            {
                Title = title,
                Summary = "A movie that needs embedding generation.",
                Year = 2024,
                Rating = 7.5m,
                MovieType = MovieType.NowShowing
            };

            db.Movies.Add(movie);
            await db.SaveChangesAsync();
            movieId = movie.Id;
        });

        return movieId;
    }

    private static Task SeedGenresAsync(params string[] names)
    {
        return TestApp.ExecuteDbContextAsync(async db =>
        {
            foreach (var name in names)
            {
                db.Genres.Add(new Genre { Name = name });
            }

            await db.SaveChangesAsync();
        });
    }
}
