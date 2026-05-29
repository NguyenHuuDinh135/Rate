# ECC for Codex CLI

This repo uses project-local Codex/ECC configuration.

## Harness

- Primary config: `.codex/config.toml`
- Agent roles: `.codex/agents/`
- Project skills: `.agents/skills/`
- Project rules: `.agents/rules/`

## Stack

- .NET 10, Aspire, ASP.NET Core Minimal APIs
- Clean Architecture with CQRS/MediatR
- EF Core, PostgreSQL, pgvector, Dapper
- Redis, RabbitMQ, Elasticsearch
- Blazor Web App and shared Razor components
- Tailwind CSS v4, Fluxor, Refit

## Common Commands

```bash
dotnet test Rate.slnx
dotnet run --project src/AppHost/AppHost.csproj
npm run tw:build
scripts/db-add-migration.sh <MigrationName>
scripts/db-migrate.sh
```

## Skill Guidance

Use local skills when a task matches:

- `dotnet-best-practices`, `aspnet-minimal-api-openapi`, `ef-core`
- `pgvector`, `postgresql`, `postgres-patterns`, `database-migrations`
- `tdd-workflow`, `verification-loop`, `e2e-testing`, `security-review`
- `deployment-patterns`, `docker-patterns`, `tailwindcss`
- `workspace-surface-audit`, `search-first`

## Security

- Validate inputs at API and application boundaries.
- Never hardcode secrets.
- Do not print secret values from appsettings, user secrets, env vars, or CI.
- Review `git diff` before push.
- Use `sandbox_mode = "workspace-write"` unless a task explicitly requires stricter read-only review.
