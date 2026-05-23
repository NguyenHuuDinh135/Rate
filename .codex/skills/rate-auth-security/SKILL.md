---
name: rate-auth-security
description: Work on Rate authentication and security-sensitive flows including ASP.NET Core Identity, JWT issuance/validation, refresh tokens, Redis token revocation, one-time tokens, password reset, authorization policies, auth endpoints, frontend login/signup forms, token storage, and protected API calls. Use for tasks touching Auth commands, Identity/Jwt infrastructure, Redis auth services, Web auth endpoints, auth context/hooks, or security reviews.
---

# Rate Auth Security

## Orientation

Auth spans backend and frontend:

- Backend commands: `src/Application/Auth/Commands`.
- Infrastructure: `Identity`, `Jwt`, Redis token/revocation/one-time-token services.
- Web: `src/Web/Endpoints/AuthEndpoints.cs` and auth middleware in `Program.cs`.
- Frontend: `app/(auth)`, `contexts/auth-context.tsx`, `hooks/use-auth.ts`, `lib/api/auth.ts`, `lib/api/api-client.ts`.

## Change Workflow

1. Keep credential verification, token creation, refresh, revoke, and password-reset logic server-side.
2. Preserve access-token-in-memory behavior in the frontend unless intentionally changing the security model.
3. Use `apiClient` for protected frontend calls so refresh handling remains centralized.
4. Make authorization explicit on endpoints with `RequireAuthorization`, policies, or anonymous access.
5. Store revocation, one-time-token, idempotency, and rate-limit state in Redis-backed services rather than ad hoc memory state.
6. Review CORS, cookie credentials, JWT settings, and secret sources together when changing auth behavior.

## Security Checks

- Do not log tokens, passwords, reset codes, or connection strings.
- Do not commit secrets in appsettings or `.env`.
- Keep refresh-token failure paths clearing client auth state.
- Add tests for expired, revoked, invalid, and unauthorized cases when auth behavior changes.
