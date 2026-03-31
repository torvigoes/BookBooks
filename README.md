# BookBooks

BookBooks is a social reading platform inspired by Letterboxd, built with .NET 9 and Clean Architecture.

## Current State (March 31, 2026)

Implemented backend/API:

- JWT auth (`register`, `login`)
- Books (`create`, `get by id`, `search`)
- Reviews (`create`, `update`, `delete`, `toggle like`, `list by book`)
- Reading status (`get/upsert by book for authenticated user`)
- Lists (`create`, `get mine`, `get by id`, `add/remove book`, `delete`)
- Follows (`follow user`, `unfollow user`, `get my following`)

Frontend:

- `BookBooks.Web` (Blazor WebAssembly) is still available as legacy UI
- Angular migration is now the official direction for next steps
- New Angular app target folder: `bookbooks-web/` (repo root, outside `.sln`)

## Tech Stack

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core 9 (SQL Server)
- ASP.NET Identity + JWT
- MediatR + FluentValidation
- Frontend migration target: Angular (standalone components + signals)

## Solution Structure

```text
BookBooks.sln
|- BookBooks.Domain
|- BookBooks.Application
|- BookBooks.Infrastructure
|- BookBooks.API
|- BookBooks.Web                 # legacy Blazor app during migration
|- BookBooks.Domain.Tests
|- BookBooks.Application.Tests
`- BookBooks.API.IntegrationTests

bookbooks-web/                   # new Angular app (to be created)
```

## Run Locally

Prerequisites:

- .NET 9 SDK
- SQL Server or LocalDB
- Node.js LTS + Angular CLI (for migration phase)

1. Configure local JWT secret:

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

4. Legacy UI (optional while migrating):

```bash
dotnet run --project BookBooks.Web
```

5. Bootstrap Angular app (first time only):

```bash
ng new bookbooks-web --routing --style=scss --standalone
```

6. Run Angular app:

```bash
cd bookbooks-web
npm install
ng serve
```

Notes:

- API development ports: `https://localhost:7007` and `http://localhost:5247`
- CORS in API already allows `http://localhost:4200`
- Add `https://localhost:4200` when Angular is served with SSL

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
- `GET /api/follows/me` (auth)
- `POST /api/follows/{followedUserId}` (auth)
- `DELETE /api/follows/{followedUserId}` (auth)

## Migration Priorities (Frontend)

1. Scaffold Angular app and core setup (routing, auth interceptor, API base config)
2. Migrate auth flow (`login`, `register`)
3. Migrate books and book details (search/create/detail + reviews)
4. Migrate reading status and lists
5. Migrate follows and then feed timeline
6. Retire Blazor screens only after feature parity and validation
