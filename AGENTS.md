# Repository Guidelines

## Project Structure & Module Organization

This is a full-stack movie booking/rating project. The backend lives in `backend/` and follows Clean Architecture: `src/Domain`, `src/Application`, `src/Infrastructure`, `src/Web`, plus Aspire orchestration in `src/AppHost`. Backend tests are under `backend/tests/*Tests`. The frontend lives in `frontend/` and uses Next.js App Router: routes in `app/`, reusable UI in `components/` and `registry/new-york-v4/ui/`, hooks in `hooks/`, API/config helpers in `lib/`, global styles in `styles/`, and Playwright checks in `tests/`.

## Build, Test, and Development Commands

- `cd backend && dotnet build backend.slnx` builds all .NET projects with warnings treated as errors.
- `cd backend && dotnet test backend.slnx` runs unit, integration, and functional test projects.
- `cd backend && dotnet run --project src/AppHost/AppHost.csproj` starts the Aspire app host and dashboard.
- `cd backend && ./scripts/db-add-migration.sh <Name>` creates an EF Core migration; `./scripts/db-migrate.sh` applies migrations.
- `cd frontend && bun run dev` starts Next.js locally.
- `cd frontend && bun run build` creates the production build.
- `cd frontend && bun run lint` runs ESLint.
- `cd frontend && BASE_URL=http://localhost:3000 bunx playwright test` runs frontend smoke tests.

## Coding Style & Naming Conventions

Backend style is defined in `backend/.editorconfig`: C# uses 4-space indentation, file-scoped namespaces, braces, nullable reference types, implicit usings, PascalCase for types/members, `I`-prefixed interfaces, and camelCase locals. Prefer adding application features under `src/Application/<Feature>/Commands` or `Queries`, endpoints under `src/Web/Endpoints`, and persistence details under `src/Infrastructure`.

Frontend TypeScript is strict and uses the `@/*` path alias. Prefer Server Components unless interactivity requires `"use client"`. Use existing Tailwind v4 utilities, `lib/api/api-client.ts` for backend calls, Zod schemas in `lib/validations`, and lucide icons/shadcn-style components from the local registry.

## Testing Guidelines

Backend tests use NUnit, Shouldly, Moq, Respawn, and Aspire testing. Name test files/classes with `*Tests` and place them in the matching project, such as `Application.UnitTests` or `Application.FunctionalTests`. Frontend tests use Playwright `*.spec.ts` files in `frontend/tests`; ensure the target Next.js server is running before browser tests.

## Commit & Pull Request Guidelines

Recent history uses short conventional-style messages such as `feat(ai): ...`, `test(backend): ...`, `chore: ...`, and `fix ...`. Keep commits focused and imperative. PRs should include a concise description, affected backend/frontend areas, test results, linked issues when applicable, and screenshots for UI changes.

## Security & Configuration Tips

Do not commit secrets. Root `.gitignore` excludes `.env`, and frontend `.gitignore` excludes `.env*`. Keep local service credentials in environment files or development appsettings only.
