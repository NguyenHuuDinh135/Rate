---
name: rate-testing-quality
description: Add, fix, or run tests and quality checks for Rate across backend NUnit/Shouldly unit, integration, and functional tests, frontend Playwright tests, linting, builds, and regression validation. Use when the user asks to verify changes, stabilize tests, improve coverage, debug CI failures, or decide the right validation set for backend/frontend/AI/data work.
---

# Rate Testing Quality

## Test Map

- Backend unit tests: `backend/tests/Application.UnitTests`, `Domain.UnitTests`.
- Backend integration tests: `backend/tests/Infrastructure.IntegrationTests`.
- Backend functional tests: `backend/tests/Application.FunctionalTests` with Aspire test host support.
- Frontend browser tests: `frontend/tests/*.spec.ts` with Playwright.

Backend tests use NUnit, Shouldly, Moq, Respawn, and coverlet packages.

## Choosing Tests

1. Domain or value object change: run the matching unit test project.
2. Application handler change: add/update application unit tests; run functional tests if API behavior changes.
3. EF, migrations, seed, Redis, or pgvector change: run integration or functional tests.
4. Endpoint/auth behavior change: run functional tests and build Web.
5. Frontend route/component change: run lint/build and Playwright for affected routes.

## Commands

```bash
cd backend && dotnet build backend.slnx
cd backend && dotnet test backend.slnx
cd frontend && bun run lint
cd frontend && bun run build
cd frontend && BASE_URL=http://localhost:3000 bunx playwright test
```

## Standards

- Name test files/classes with `*Tests`.
- Prefer assertions that describe behavior, not implementation detail.
- Keep warnings clean because backend builds treat warnings as errors.
- Do not skip failing tests without documenting the exact external dependency or environment limitation.
