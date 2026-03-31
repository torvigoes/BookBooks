# BookBooks

BookBooks is a social reading platform inspired by Letterboxd, built with .NET 9 and Clean Architecture.

## Current State (March 31, 2026)

Implemented in this repository:

- Backend API with JWT authentication (`register`, `login`)
- Books endpoints (`create`, `get by id`)
- Reviews endpoints (`create`, `update`, `delete`, `toggle like`, `list by book`)
- EF Core + SQL Server persistence with initial migration
- Blazor WebAssembly project scaffolded in solution

Not implemented yet in code:

- Reading status flows
- Lists management endpoints
- Follow/feed endpoints
- Full frontend experience (current Blazor app is still template-level)
- Automated tests

## Tech Stack

- .NET 9
- ASP.NET Core Web API
- Blazor WebAssembly
- Entity Framework Core (SQL Server)
- ASP.NET Identity + JWT
- MediatR + FluentValidation

## Solution Structure

```text
BookBooks.sln
|- BookBooks.Domain          # Entities, enums, interfaces, Result
|- BookBooks.Application     # Commands/queries (CQRS), validators, handlers
|- BookBooks.Infrastructure  # EF Core, repositories, JWT provider
|- BookBooks.API             # Controllers, DI, Swagger
`- BookBooks.Web             # Blazor WebAssembly client (in progress)
```

## Run Locally

Prerequisites:

- .NET 9 SDK
- SQL Server or LocalDB

1. Configure connection string and JWT values.

`BookBooks.API/appsettings.json` already contains local defaults. For development, prefer overriding via:

- `BookBooks.API/appsettings.Development.json`
- `dotnet user-secrets`
- environment variables

2. Apply migrations:

```bash
dotnet ef database update --project BookBooks.Infrastructure --startup-project BookBooks.API
```

3. Run API:

```bash
dotnet run --project BookBooks.API
```

4. Run Web:

```bash
dotnet run --project BookBooks.Web
```

## API Quick Reference

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/books`
- `GET /api/books/{id}`
- `POST /api/books/{bookId}/reviews` (auth)
- `PUT /api/reviews/{reviewId}` (auth)
- `DELETE /api/reviews/{reviewId}` (auth)
- `POST /api/reviews/{reviewId}/like` (auth)
- `GET /api/books/{bookId}/reviews`

Swagger is available in development mode.

## Development Backlog

Priority 1:

- Add automated tests (Domain, Application, API integration)
- Deliver first real Blazor flow (auth + books + reviews)
- Move JWT secret out of tracked config for non-local environments

Priority 2:

- Lists feature (commands, queries, endpoints, UI)
- Reading status feature (upsert + retrieval + UI)
- Follow/feed feature

Priority 3:

- Notifications
- Reading analytics dashboard
- External book metadata sync strategy
