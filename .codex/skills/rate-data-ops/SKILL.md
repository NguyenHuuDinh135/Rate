---
name: rate-data-ops
description: Work on Rate data persistence, EF Core model configuration, PostgreSQL/pgvector mappings, migrations, JSON seed data, database initialization, Redis-backed services, and local database scripts. Use for tasks touching ApplicationDbContext, Infrastructure/Data, SeedData, Migrations, db scripts, cache/idempotency/rate-limit storage, or data-related functional tests.
---

# Rate Data Ops

## Orientation

Persistence lives mostly in `backend/src/Infrastructure/Data`:

- `ApplicationDbContext.cs`: DbSets and EF model entry point.
- `Configurations/*.cs`: per-entity mapping.
- `Migrations/`: generated EF migrations.
- `SeedData/*.json`: local/dev seed data.
- `ApplicationDbContextInitialiser.cs`: database creation, migrations, and seed loading.
- `scripts/db-add-migration.sh` and `scripts/db-migrate.sh`: local/CI migration helpers.

## Change Workflow

1. Update domain entity and application contracts first.
2. Add or adjust EF configuration in `Configurations`.
3. Update `ApplicationDbContext` only when a new aggregate/table needs a DbSet.
4. Generate a migration with a descriptive name; do not hand-edit snapshots unless repairing an EF generation issue.
5. Keep seed JSON schema aligned with entity/configuration requirements.
6. For pgvector changes, verify Npgsql vector setup in infrastructure DI and the `pgvector/pgvector:pg16` Aspire container.

## Commands

Run from `backend/`:

```bash
./scripts/db-add-migration.sh <MigrationName>
./scripts/db-migrate.sh
dotnet test tests/Infrastructure.IntegrationTests/Infrastructure.IntegrationTests.csproj
```

## Cautions

Seed data, migrations, and functional tests are tightly coupled. When changing required fields, update JSON seed files and affected tests in the same change.
