# Rate Project Rules

- Treat `src/WebUI/Shared` as the shared UI ownership boundary for web and MAUI unless a host-specific change is required.
- Keep application behavior in `src/Application`, persistence and external services in `src/Infrastructure`, and endpoint wiring in `src/Web`.
- Use MediatR/CQRS patterns already present in the repo for new commands and queries.
- Add or update focused tests for behavioral changes. Prefer existing test projects before creating new ones.
- Never hardcode secrets. Use configuration keys, user secrets, environment variables, or Azure Key Vault.
- For EF Core schema changes, create migrations under `src/Infrastructure/Data/Migrations` and verify startup/migration behavior.
- For auth, payment, idempotency, Redis, SMTP, AI provider, or Key Vault changes, run a security review before commit.
- Do not print values for `JwtSettings:SecretKey`, SMTP credentials, AI API keys, connection strings, or Azure credentials.
