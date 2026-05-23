---
name: rate-frontend-next
description: Work on the Rate frontend under frontend for Next.js App Router, React components, route groups, Tailwind v4 styling, shadcn-style registry components, auth UI, API calls, and Playwright smoke tests. Use for frontend feature work, UI fixes, state/data fetching changes, and reviews touching frontend/app, components, hooks, lib, styles, or tests.
---

# Rate Frontend Next

## Orientation

The frontend is a Next.js 16 App Router app in `frontend/`.

- `app/`: route groups such as `(app)` and `(auth)`.
- `components/`: shared app components.
- `registry/new-york-v4/ui/`: local shadcn-style primitives.
- `hooks/`, `contexts/`, `types/`: reusable client state and types.
- `lib/api`, `lib/constants`, `lib/validations`: API, config, and Zod validation.
- `styles/globals.css`: Tailwind v4 theme and global styling.

## Change Workflow

1. Prefer Server Components. Add `"use client"` only for hooks, browser APIs, form state, or interactive UI.
2. Use existing registry primitives before creating new UI components.
3. Route backend calls through `lib/api/api-client.ts` when auth/refresh behavior matters.
4. Keep API URLs centralized through `API_CONFIG` and auth routes through `AUTH_CONFIG`.
5. Put schemas in `lib/validations` and shared types in `types/`.
6. Preserve App Router route groups; add pages under the nearest existing group.

## UI Rules

- Use Tailwind utilities and CSS variables from the project theme.
- Use lucide icons where available; keep Hugeicons usage consistent with existing components.
- Keep text responsive and avoid layout shifts in compact navigation, cards, forms, and chat surfaces.
- For AI chat changes, coordinate with backend `/api/ai/*` behavior and the `@ai-sdk/react` `useChat` contract.

## Verification

Run from `frontend/`:

```bash
bun run lint
bun run build
BASE_URL=http://localhost:3000 bunx playwright test
```
