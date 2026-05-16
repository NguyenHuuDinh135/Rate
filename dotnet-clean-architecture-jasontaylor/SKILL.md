---
name: dotnet-clean-architecture-jasontaylor
description: Specialized guidance for building features using Jason Taylor's .NET Clean Architecture template. Use this skill whenever the user mentions adding features, commands, queries, handlers, entities, or web endpoints in the backend. It ensures strict adherence to Clean Architecture principles, CQRS patterns, MediatR usage, and Minimal API structures as implemented in this specific codebase.
---

# dotnet-clean-architecture-jasontaylor

Help build features in a .NET solution following Jason Taylor's Clean Architecture template.

## Architecture Overview
- **Domain**: Entities, Value Objects, Enums, Exceptions, Domain Events.
- **Application**: CQRS (Commands/Queries), MediatR Handlers, DTOs, Interfaces, Validators.
- **Infrastructure**: Persistence (EF Core), Identity, External Services.
- **Web**: Minimal APIs (EndpointGroups), Dependency Injection.

## Coding Patterns
- **CQRS**: Command/Query and Handler are often kept in the same file or directory (`Application/[Feature]/[Commands|Queries]/[Name]/`).
- **Primary Constructors**: Use primary constructors for dependency injection in Handlers and Services.
- **Minimal APIs**: Use `IEndpointGroup` to define and map endpoints in the `Web/Endpoints` directory.
- **Result Pattern**: Use `Result<T>` or `Result` for Command/Query responses.

## CLI Scaffolding
The template provides a `ca-usecase` item template to scaffold new features.
- **Install (if missing)**: `dotnet new install Clean.Architecture.Solution.Template`
- **Add a Command**:
  ```bash
  dotnet new ca-usecase -fn <FeatureName> -n <CommandName> -ut command -rt <ReturnType>
  ```
- **Add a Query**:
  ```bash
  dotnet new ca-usecase -fn <FeatureName> -n <QueryName> -ut query -rt <ReturnType>
  ```
*Note: Run these commands from within the `src/Application` directory or specify the `--output` path.*

## Instructions
1.  **Research First**: Before adding a feature, identify the Domain Entity and required operations (CRUD, Search, etc.).
2.  **Scaffold with CLI**: Prefer using `dotnet new ca-usecase` to generate the initial structure for Commands and Queries in `Application/`.
3.  **Entity First**: Add or update the Domain Entity in `Domain/Entities/`.
4.  **Application Logic**:
    -   Refine the scaffolded Commands/Queries.
    -   Use `record` for Request objects.
    -   Use `sealed class` with Primary Constructor for Handlers.
4.  **Endpoints**:
    -   Create a new EndpointGroup in `Web/Endpoints/`.
    -   Register the endpoint group in `DependencyInjection.cs` if necessary (usually automated via reflection if following the template's pattern).
5.  **Validation**: Add FluentValidation rules in `Application/[Feature]/Commands/[Name]/[Name]Validator.cs` if needed.
