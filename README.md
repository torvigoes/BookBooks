# BookBooks

BookBooks is a social reading platform inspired by Letterboxd, built with .NET 9 and Clean Architecture.

## Current State (March 31, 2026)

Implemented:

- JWT auth (`register`, `login`)
- Books (`create`, `get by id`, `search`)
- Reviews (`create`, `update`, `delete`, `toggle like`, `list by book`)
- Reading status (`get/upsert by book for authenticated user`)
- Lists (`create`, `get mine`, `get by id`, `add/remove book`, `delete`)
- Blazor Web flow:
  - login/register
  - books search/create/detail
  - reviews flow
  - reading status flow
  - lists flow
- Automated tests:
  - Domain unit tests
  - Application unit tests
  - API integration tests (auth guards/smoke)

Still pending:

- Follow/feed module
- Notifications
- Reading analytics dashboard
- External metadata sync strategy (Open Library)

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
|- BookBooks.Web             # Blazor WebAssembly client
|- BookBooks.Domain.Tests
|- BookBooks.Application.Tests
`- BookBooks.API.IntegrationTests
```

## Run Locally

Prerequisites:

- .NET 9 SDK
- SQL Server or LocalDB

1. Configure local JWT secret (required in non-test runtime):

```bash
dotnet user-secrets --project BookBooks.API set "JwtOptions:SecretKey" "your-strong-local-secret"
```

2. Apply migrations:

```bash
dotnet ef database update --project BookBooks.Infrastructure --startup-project BookBooks.API
```

3. Run API:

```bash
dotnet run --project BookBooks.API --launch-profile https
```

4. Run Web:

```bash
dotnet run --project BookBooks.Web
```

Notes:

- If port `5247` is already in use, stop old API process instances before rerunning.
- Web chooses API base URL automatically based on HTTP/HTTPS host profile.

## API Quick Reference

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/books`
- `GET /api/books/{id}`
- `GET /api/books?searchTerm=&page=1&pageSize=20`
- `POST /api/books/{bookId}/reviews` (auth)
- `PUT /api/reviews/{reviewId}` (auth)
- `DELETE /api/reviews/{reviewId}` (auth)
- `POST /api/reviews/{reviewId}/like` (auth)
- `GET /api/books/{bookId}/reviews`
- `GET /api/books/{bookId}/reading-status` (auth)
- `PUT /api/books/{bookId}/reading-status` (auth)
- `POST /api/lists` (auth)
- `GET /api/lists` (auth)
- `GET /api/lists/{listId}` (auth)
- `POST /api/lists/{listId}/books` (auth)
- `DELETE /api/lists/{listId}/books/{bookId}` (auth)
- `DELETE /api/lists/{listId}` (auth)

Swagger is available in development mode.

## Development Backlog

Priority 1:

- Expand tests for new slices (`Lists`, `ReadingStatus`, `SearchBooks`) with deeper scenarios
- Improve UI feedback/observability (API online status, loading/error consistency)

Priority 2:

- Follow/feed feature (commands, queries, endpoints, Web UI)
- Lists enhancements (ordering, editing metadata, pagination)

Priority 3:

- Notifications
- Reading analytics dashboard
- Open Library integration pipeline
