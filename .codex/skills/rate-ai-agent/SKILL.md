---
name: rate-ai-agent
description: Work on Rate AI assistant features including AI chat endpoints, Semantic Kernel services, Gemini/OpenAI/Ollama provider configuration, embeddings, pgvector movie search, prompt management, AI plugins, audit/session/message tables, and the frontend AI chat page. Use for tasks touching backend/src/Application/AI, Infrastructure/AI, Web/Endpoints/AiEndpoints.cs, vector search queries, embedding sync, or frontend/app/(app)/ai-chat.
---

# Rate AI Agent

## Orientation

AI features span backend and frontend:

- `src/Infrastructure/AI/DependencyInjection.cs`: provider selection, Kernel setup, plugins.
- `src/Infrastructure/AI/SemanticKernel`: AI service implementation.
- `src/Application/AI`: interfaces and plugins.
- `src/Application/Movies/Queries/SearchMoviesByVector`: pgvector semantic search.
- `src/Infrastructure/AI/BackgroundJobs`: embedding synchronization.
- `src/Web/Endpoints/AiEndpoints.cs`: chat/session API surface.
- `frontend/app/(app)/ai-chat/page.tsx`: `@ai-sdk/react` chat UI.

## Change Workflow

1. Keep provider-specific logic behind `ILLMProvider`, `IEmbeddingProvider`, `IAIService`, or `IPromptManager`.
2. Register plugins in both DI and Kernel construction when adding Semantic Kernel capabilities.
3. Preserve existing configuration keys unless deliberately migrating them; this code currently mixes `AI:*` and `Ai:*` keys.
4. Never commit API keys. Use `GEMINI_API_KEY`, Aspire environment wiring, or local appsettings ignored from source.
5. For vector search, verify embedding dimensions, `Pgvector` mappings, EF migrations, and null-embedding behavior.
6. Keep chat response shape compatible with `useChat` before changing frontend streaming or message handling.

## Verification

Use focused backend tests for AI/vector changes, then build:

```bash
cd backend
dotnet test tests/Application.FunctionalTests/Application.FunctionalTests.csproj
dotnet build backend.slnx
```
