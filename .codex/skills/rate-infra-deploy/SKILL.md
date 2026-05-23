---
name: rate-infra-deploy
description: Work on Rate infrastructure, local orchestration, and deployment configuration including .NET Aspire AppHost, Azure Developer CLI files, Bicep templates, service connection names, appsettings, Key Vault integration, Docker-backed local services, and deployment readiness. Use for tasks touching backend/src/AppHost, backend/infra, backend/azure.yaml, appsettings, service defaults, or cloud/runtime configuration.
---

# Rate Infra Deploy

## Orientation

Local orchestration is in `backend/src/AppHost/Program.cs`. It currently wires:

- PostgreSQL using `pgvector/pgvector:pg16` on port `54322`.
- Redis on port `6379`.
- RabbitMQ with management plugin.
- Elasticsearch.
- Web API with references to database, Redis, messaging, Elasticsearch, and `GEMINI_API_KEY`.

Azure deployment assets live in `backend/infra/` and `backend/azure.yaml`.

## Change Workflow

1. Keep service names consistent with `backend/src/Shared/Services.cs`, connection strings, and `WithReference` usage.
2. When adding a local service, update AppHost, Web/Infrastructure DI, appsettings, and tests that depend on the service.
3. When adding cloud resources, update Bicep modules and parameters together; keep secrets in Key Vault or environment-backed settings.
4. Do not run `azd up` or change live cloud resources unless the user explicitly asks.
5. Preserve stable local volumes unless the task is specifically to reset local state.

## Commands

```bash
cd backend && dotnet run --project src/AppHost/AppHost.csproj
cd backend && dotnet build backend.slnx
cd backend && azd auth login
cd backend && azd up
```

Only use the `azd` commands when deployment is intended and credentials are available.
