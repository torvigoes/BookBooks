# BookBooks - Architecture Reference

## Project Overview

BookBooks is a social reading platform with Clean Architecture.

- Backend: .NET 9 / C# 13
- Database: SQL Server + EF Core 9 (code first)
- Auth: ASP.NET Core Identity + JWT
- API: REST JSON + RFC 7807 style error responses
- Frontend target: Angular (new)
- Frontend legacy: Blazor WebAssembly (`BookBooks.Web`) during migration

## Current Migration State

- Backend is active and stable (`Domain`, `Application`, `Infrastructure`, `API`)
- Blazor frontend is still functional and treated as legacy
- Angular frontend will live at `bookbooks-web/` in the repo root
- Migration strategy is incremental by feature (not big-bang)

## Solution Structure

```text
BookBooks.sln
|- BookBooks.Domain
|- BookBooks.Application
|- BookBooks.Infrastructure
|- BookBooks.API
|- BookBooks.Web                 # legacy frontend during migration
|- BookBooks.Domain.Tests
|- BookBooks.Application.Tests
`- BookBooks.API.IntegrationTests

bookbooks-web/                   # new Angular frontend (outside .sln)
```

## Dependency Rules

- `Domain` -> no dependencies on other projects
- `Application` -> depends only on `Domain`
- `Infrastructure` -> depends on `Domain` + `Application`
- `API` -> depends on `Application` + `Infrastructure`
- `bookbooks-web` -> depends only on API HTTP contracts

## Backend Feature Baseline

Implemented endpoints include:

- Auth: register/login
- Books: create/get/search
- Reviews: create/update/delete/toggle like/list by book
- Reading status: get/upsert by book
- Lists: create/get mine/get by id/add/remove/delete
- Follows: follow/unfollow/get my following
- Feed: authenticated timeline from followed users

Still pending:

- Notifications
- Reading analytics dashboard
- Open Library integration pipeline

## Frontend Direction (Angular)

Angular app conventions:

- Standalone components
- Signals for local state
- Dedicated API services for all HTTP access
- Auth interceptor for bearer token
- Route guards for protected routes

Suggested top-level app structure:

```text
bookbooks-web/src/app/
|- core/
|  |- auth/
|  |- api/
|  `- models/
|- features/
|  |- auth/
|  |- books/
|  |- reviews/
|  |- lists/
|  |- follows/
|  `- feed/
`- shared/
```

## CORS and Local Development

API must allow local frontend origins:

- `http://localhost:4200`
- `https://localhost:4200`
- existing Blazor dev origins while migration is in progress

## Implementation Rules

- Keep `Result<T>` pattern in `Application`; avoid exceptions for business flow
- Keep domain invariants in constructors/methods
- Keep DTOs as records in `Application`
- Keep validators close to commands/queries
- Keep controllers thin and mediated by MediatR
- Keep Blazor changes minimal and migration-focused only

## Definition of Done for Each Migrated Feature

1. Angular screen/flow implemented
2. Angular API service contracts aligned with backend DTOs
3. Auth/authorization behavior preserved
4. Legacy Blazor route can be retired or flagged as deprecated
5. README and migration plan updated
