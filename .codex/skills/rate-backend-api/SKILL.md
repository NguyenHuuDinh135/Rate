---
name: rate-backend-api
description: Work on the Rate backend under backend/src for .NET Clean Architecture, CQRS/MediatR handlers, Minimal API endpoint groups, domain entities, infrastructure services, authentication, payments, caching, and API contracts. Use for backend feature work, bug fixes, refactors, and reviews touching C# project files in backend/src or backend/tests.
---

# Rate Backend API

## Orientation

The backend is a .NET 10 Clean Architecture solution in `backend/`. Main layers:

- `src/Domain`: entities, enums, constants, value objects, domain primitives.
- `src/Application`: CQRS commands/queries, interfaces, behaviours, models, validation.
- `src/Infrastructure`: EF Core, Identity/JWT, Redis, Hangfire, AI, payments, Dapper.
- `src/Web`: Minimal API endpoint groups, app startup, exception handling, OpenAPI/Scalar.

## Change Workflow

1. Find the closest existing feature and mirror its shape before adding abstractions.
2. Put application use cases in `src/Application/<Feature>/Commands` or `Queries`.
3. Use `IApplicationDbContext` and application interfaces from handlers; keep EF and provider details in `Infrastructure`.
4. Add or update endpoints in `src/Web/Endpoints/*Endpoints.cs` using `IEndpointGroup`, `ISender`, `TypedResults`, and explicit auth requirements.
5. Register new infrastructure implementations in `src/Infrastructure/DependencyInjection.cs` or the relevant feature DI file.
6. Keep domain types persistence-agnostic unless the existing entity model already requires a persistence-specific value.

## Local Patterns

- Commands commonly return `Result` or `Result<T>`; queries return DTOs or read models.
- Endpoint prefixes use `/api/<resource>` and methods delegate to MediatR.
- Backend warnings are errors; fix analyzer/compiler warnings instead of suppressing them.
- Do not put secrets in `appsettings*.json`. Prefer environment variables, Aspire parameters, or Key Vault wiring.

## Verification

Run from `backend/`:

```bash
dotnet build backend.slnx
dotnet test backend.slnx
```
