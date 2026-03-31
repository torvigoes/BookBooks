# BookBooks - Copilot Instructions

## Context

BookBooks follows Clean Architecture with a .NET 9 backend and a frontend migration in progress:

- Legacy frontend: `BookBooks.Web` (Blazor WebAssembly)
- Target frontend: `bookbooks-web/` (Angular, standalone, signals)

Backend is the source of truth. Frontend work must follow existing API contracts.

## Architecture Rules

- `BookBooks.Domain`: no dependencies on other projects
- `BookBooks.Application`: depends only on Domain
- `BookBooks.Infrastructure`: depends on Domain + Application
- `BookBooks.API`: depends on Application + Infrastructure
- `bookbooks-web`: consume API only via HTTP

Do not violate dependency direction.

## Backend Coding Rules

- Keep `Result<T>` pattern in Application for business flow
- Keep validators next to commands/queries
- Keep DTOs as records
- Keep domain invariants in entity constructors/methods
- Keep controllers thin and MediatR-driven
- Preserve nullable reference types and cancellation tokens

## Frontend Migration Rules

- Prefer implementing new UI in Angular, not Blazor
- Touch Blazor only for hotfixes or migration support
- Angular conventions:
  - standalone components
  - signals for local state
  - API services in `core/api`
  - auth interceptor + route guards

## API Contract Guidance

Current implemented slices include:

- auth
- books
- reviews
- reading status
- lists
- follows

Pending major slice: feed timeline.

Any contract changes must be reflected in:

- `README.md`
- `ARCHITECTURE.md`
- Angular models/services when scaffolded

## CORS and Local Dev

Keep API CORS ready for:

- `http://localhost:4200`
- `https://localhost:4200`
- existing local Blazor origins during migration

## Quality Bar

- Add/adjust tests whenever behavior changes
- Keep commits scoped and follow Conventional Commits
- Do not introduce unrelated refactors in feature changes
