using backend.Application.AI.Interfaces;
using backend.Application.AI.Plugins;
using backend.Infrastructure.AI.Prompts;
using backend.Infrastructure.AI.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.AI;

namespace backend.Infrastructure.AI;

public static class DependencyInjection
{
    public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration configuration)
    {
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0070
        var openAiApiKey = configuration["AI:OpenAI:ApiKey"];
        var openAiModelId = configuration["AI:OpenAI:ModelId"] ?? "gpt-4o-mini";
        
        var geminiApiKey = configuration["Ai:Gemini:ApiKey"];
        var geminiModelId = configuration["Ai:Gemini:ModelId"] ?? "gemini-1.5-flash";

        var kernelBuilder = Kernel.CreateBuilder();
        
        if (!string.IsNullOrEmpty(geminiApiKey))
        {
            kernelBuilder.AddGoogleAIGeminiChatCompletion(geminiModelId, geminiApiKey);
        }
        else if (!string.IsNullOrEmpty(openAiApiKey))
        {
            kernelBuilder.AddOpenAIChatCompletion(openAiModelId, openAiApiKey);
            kernelBuilder.AddOpenAITextEmbeddingGeneration("text-embedding-3-small", openAiApiKey);
        }

        // Add Providers
        services.AddScoped<backend.Application.Common.Interfaces.AI.ILLMProvider, Providers.GeminiLLMProvider>();
        
        // Ollama Embedding Configuration
        var ollamaEndpoint = configuration["AI:Ollama:Endpoint"] ?? "http://localhost:11434";
        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp => 
            new OllamaEmbeddingGenerator(new Uri(ollamaEndpoint), "bge-m3"));
        services.AddScoped<backend.Application.Common.Interfaces.AI.IEmbeddingProvider, Providers.OllamaEmbeddingProvider>();

        // Add Plugins
        services.AddScoped<MovieBookingPlugin>();
        
        services.AddTransient(sp => 
        {
            var kernel = kernelBuilder.Build();
            kernel.Plugins.AddFromObject(sp.GetRequiredService<MovieBookingPlugin>());
            return kernel;
        });
        
        services.AddScoped<IAIService, SemanticKernelService>();
        services.AddScoped<IPromptManager, DatabasePromptManager>();

        return services;
#pragma warning restore SKEXP0010
#pragma warning restore SKEXP0070
    }
}
