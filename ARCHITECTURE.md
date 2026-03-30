# BookBooks — Architecture Reference

> This file is the primary context document for GitHub Copilot.
> Keep it open in a tab while developing to improve suggestion quality.

---

## Project Overview

**BookBooks** is a book-tracking social platform inspired by Letterboxd, built with ASP.NET Core 8.
Users can catalog books they have read, write reviews with ratings, organize books into lists,
and follow other users to see their reading activity.

**Backend language:** C# 12 / .NET 8  
**Frontend:** Angular 17+ (standalone components, signals)  
**Database:** SQL Server (EF Core 8, Code First)  
**Auth:** ASP.NET Core Identity + JWT Bearer tokens  
**API style:** RESTful JSON API  
**Communication:** Angular HttpClient → ASP.NET Core Web API  

---

## Solution Structure

```
BookBooks.sln
├── BookBooks.Domain          → Core entities, enums, domain interfaces, domain events
├── BookBooks.Application     → Use cases, DTOs, service interfaces, validators, mappings
├── BookBooks.Infrastructure  → EF Core DbContext, repositories, external APIs, email
├── BookBooks.API             → ASP.NET Core Web API, controllers, middleware, DI setup
└── bookbooks-web/            → Angular app (separate folder, NOT a .csproj project)
```

### Note on the Angular project

The Angular app (`bookbooks-web/`) lives in the same repository root but is **not part of the .sln**.
It is a standalone Node/Angular project managed with the Angular CLI.
It communicates with `BookBooks.API` exclusively via HTTP (REST + JSON).
CORS is configured in `BookBooks.API` to allow requests from `http://localhost:4200` in development.

### Dependency Rules (Clean Architecture)

- **Domain** has zero dependencies on other projects or NuGet packages
- **Application** depends only on Domain
- **Infrastructure** depends on Domain and Application (implements their interfaces)
- **API** depends on Application and Infrastructure (wires everything together via DI)
- **bookbooks-web** depends only on the API's HTTP contract (OpenAPI/Swagger)

---

## Domain Layer — `BookBooks.Domain`

### Core Entities

```
Entities/
├── AppUser.cs          → extends IdentityUser, adds profile fields (bio, avatarUrl, username)
├── Book.cs             → bibliographic data, average rating cache
├── Review.cs           → user review for a book (rating 1–5, text, spoiler flag)
├── BookList.cs         → named user-curated list of books
├── BookListItem.cs     → join entity: BookList ↔ Book (with order/notes)
├── UserFollow.cs       → composite key: FollowerId + FollowedId
├── ReviewLike.cs       → composite key: UserId + ReviewId
└── ReadingStatus.cs    → tracks per-user status per book (Want/Reading/Read/Abandoned)
```

### Key Enums

```csharp
public enum ReadingStatusType { WantToRead, CurrentlyReading, Read, Abandoned }
public enum ListVisibility    { Public, Private, FriendsOnly }
```

### Domain Interfaces (implemented in Infrastructure)

```csharp
IBookRepository       // CRUD + search books
IReviewRepository     // get reviews by book, by user, feed
IBookListRepository   // manage user lists
IUserRepository       // follow/unfollow, search users
IUnitOfWork           // wraps DbContext.SaveChangesAsync()
```

---

## Application Layer — `BookBooks.Application`

Organized by **feature folder**, not by type:

```
Features/
├── Books/
│   ├── Commands/   → CreateBook, UpdateBook
│   ├── Queries/    → GetBookById, SearchBooks, GetBookFeed
│   └── DTOs/       → BookDto, BookSummaryDto, BookSearchResultDto
├── Reviews/
│   ├── Commands/   → CreateReview, UpdateReview, DeleteReview, LikeReview
│   ├── Queries/    → GetReviewsByBook, GetReviewsByUser, GetReviewFeed
│   └── DTOs/       → ReviewDto, CreateReviewRequest, ReviewSummaryDto
├── Lists/
│   ├── Commands/   → CreateList, AddBookToList, RemoveBookFromList, DeleteList
│   ├── Queries/    → GetListById, GetUserLists
│   └── DTOs/       → BookListDto, BookListSummaryDto
├── Users/
│   ├── Commands/   → FollowUser, UnfollowUser, UpdateProfile
│   ├── Queries/    → GetUserProfile, GetUserFeed, GetFollowers, GetFollowing
│   └── DTOs/       → UserProfileDto, UserSummaryDto, ActivityFeedItemDto
└── Auth/
    ├── Commands/   → Register, Login, RefreshToken
    └── DTOs/       → AuthRequest, AuthResponse, RegisterRequest
```

### Validation

- All `Commands` and `Queries` use **FluentValidation**
- Validators live next to their request class (e.g. `CreateReviewValidator.cs`)

### Mapping

- **Mapster** maps between Entities ↔ DTOs
- Mapping profiles are defined per feature folder

---

## Infrastructure Layer — `BookBooks.Infrastructure`

```
Persistence/
├── AppDbContext.cs              → IdentityDbContext<AppUser>, all DbSets
├── Configurations/              → IEntityTypeConfiguration<T> per entity (Fluent API)
├── Migrations/                  → EF Core migrations (auto-generated)
└── Repositories/                → concrete implementations of domain interfaces

ExternalServices/
├── OpenLibraryService.cs        → fetches book metadata from Open Library API
└── CoverImageService.cs         → resolves book cover URLs
```

### DbContext DbSets

```csharp
DbSet<Book>          Books
DbSet<Review>        Reviews
DbSet<ReviewLike>    ReviewLikes
DbSet<BookList>      BookLists
DbSet<BookListItem>  BookListItems
DbSet<UserFollow>    UserFollows
DbSet<ReadingStatus> ReadingStatuses
```

### Composite Keys (configured via Fluent API)

```csharp
// UserFollow
entity.HasKey(f => new { f.FollowerId, f.FollowedId });

// ReviewLike
entity.HasKey(l => new { l.UserId, l.ReviewId });

// BookListItem
entity.HasKey(i => new { i.BookListId, i.BookId });
```

---

## API Layer — `BookBooks.API`

```
Controllers/
├── AuthController.cs       → POST /api/auth/register, /api/auth/login, /api/auth/refresh
├── BooksController.cs      → GET/POST /api/books, GET /api/books/{id}
├── ReviewsController.cs    → GET/POST /api/books/{id}/reviews, PUT/DELETE /api/reviews/{id}
├── ListsController.cs      → CRUD /api/lists, POST /api/lists/{id}/books
└── UsersController.cs      → GET /api/users/{username}, POST /api/users/{id}/follow

Middleware/
├── ExceptionHandlingMiddleware.cs   → global error handler, returns RFC 7807 ProblemDetails
└── CurrentUserMiddleware.cs         → resolves AppUser from JWT claims
```

### CORS Configuration

In development, the API allows requests from the Angular dev server:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});
```

### Auth Flow

- Register/Login → returns **JWT access token** (15 min) + **refresh token** (7 days)
- Angular stores the access token in **memory** (not localStorage) and the refresh token in an **HttpOnly cookie**
- All protected endpoints require `[Authorize]` and `Authorization: Bearer {token}` header
- Angular `AuthInterceptor` automatically attaches the token to every outgoing request and handles 401 → refresh flow
- Current user injected server-side via `ICurrentUserService` (reads from HttpContext claims)

### Response Conventions

- Success: `200 OK` with DTO, or `201 Created` with Location header
- Validation error: `400 Bad Request` with FluentValidation error list
- Not found: `404 Not Found` with ProblemDetails
- Unauthorized: `401` / Forbidden: `403`
- All list endpoints are **paginated**: `?page=1&pageSize=20`

---

## Frontend — `bookbooks-web` (Angular 17+)

### Setup

```bash
# Create the Angular project at the repo root
ng new bookbooks-web --routing --style=scss --standalone
cd bookbooks-web
ng add @angular/material   # optional UI component library
```

### Folder Structure

```
bookbooks-web/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── auth/             → AuthService, AuthInterceptor, authGuard
│   │   │   ├── services/         → BookService, ReviewService, UserService, ListService
│   │   │   └── models/           → TypeScript interfaces mirroring API DTOs
│   │   ├── features/
│   │   │   ├── books/            → book search, book detail, add review
│   │   │   ├── profile/          → user profile, followers, lists
│   │   │   ├── feed/             → activity feed
│   │   │   ├── lists/            → list detail and management
│   │   │   └── auth/             → login and register pages
│   │   └── shared/
│   │       └── components/       → star-rating, book-card, avatar, pagination
│   └── environments/
│       ├── environment.ts         → apiUrl: 'http://localhost:5000'
│       └── environment.prod.ts    → apiUrl: 'https://api.bookbooks.app'
```

### Angular Conventions

- Use **standalone components** (no NgModules)
- Use **signals** for local reactive state (`signal()`, `computed()`, `effect()`)
- Use **RxJS** only for HTTP streams and complex async flows
- All API calls go through dedicated services in `core/services/`
- Route guards (`authGuard`) protect routes that require authentication
- `AuthInterceptor` attaches JWT and handles 401 → token refresh automatically

### TypeScript models mirror API DTOs exactly

```typescript
// mirrors BookDto from BookBooks.Application
export interface BookDto {
  id: string;
  title: string;
  author: string;
  isbn: string;
  year: number;
  coverImageUrl: string;
  averageRating: number;
  reviewCount: number;
}

// mirrors ReviewDto from BookBooks.Application
export interface ReviewDto {
  id: string;
  bookId: string;
  userId: string;
  userDisplayName: string;
  rating: number;
  content: string;
  containsSpoiler: boolean;
  createdAt: string;
  likeCount: number;
  likedByCurrentUser: boolean;
}
```

---

## Key Business Rules

1. A user can only write **one review per book** — enforce at service + DB unique index level
2. A user cannot follow themselves
3. Deleting a review also deletes all its likes (cascade)
4. `Book.AverageRating` is a **cached computed field** — recalculated after each review insert/update/delete
5. Lists with `ListVisibility.Private` are never returned to other users
6. `ReadingStatus` is upserted (insert or update) — one record per user+book pair
7. Book ISBN must be **unique** in the database
8. Reviews must have a `Rating` between 1 and 5 and `Content` between 10 and 5000 characters

---

## External Integrations

| Service | Purpose | Key Class |
|---|---|---|
| Open Library API | Fetch book metadata by ISBN or title search | `OpenLibraryService` |
| Open Library Covers | Book cover images by ISBN | `CoverImageService` |

Open Library base URL: `https://openlibrary.org`  
Cover images: `https://covers.openlibrary.org/b/isbn/{ISBN}-L.jpg`

---

## Naming Conventions

### C# (backend)
- **Entities:** singular PascalCase (`Book`, `Review`, `BookList`)
- **DTOs:** suffix with `Dto` or `Request`/`Response` (`BookDto`, `CreateReviewRequest`)
- **Interfaces:** prefix with `I` (`IBookRepository`, `ICurrentUserService`)
- **Commands/Queries:** verb + noun + Command/Query (`CreateReviewCommand`, `GetBookByIdQuery`)
- **Controllers:** plural resource name (`BooksController`, `ReviewsController`)

### TypeScript (frontend)
- **Interfaces:** PascalCase, no `I` prefix (`BookDto`, `UserProfileDto`)
- **Services:** PascalCase class, camelCase token (`BookService`, provided in root)
- **Components:** kebab-case selector (`app-book-card`, `app-star-rating`)
- **Files:** kebab-case (`book-card.component.ts`, `auth.service.ts`)
- **Guards:** camelCase function (`authGuard`, `adminGuard`)

---

## Development Notes for Copilot

- Always use **async/await** and `CancellationToken` in C# service and repository methods
- Prefer **records** for C# DTOs and immutable value objects
- Use **pattern matching** and **switch expressions** where appropriate
- Use **nullable reference types** — all projects have `<Nullable>enable</Nullable>`
- Use `Result<T>` pattern for operation results instead of throwing exceptions in the Application layer
- Entity constructors should enforce invariants — avoid public setters on domain entities
- Write **XML doc comments** on all public C# interfaces and DTOs
- Angular components must be **standalone** — never use NgModule declarations
- Angular services should use `inject()` function instead of constructor injection
- Prefer **signals** over BehaviorSubject for simple shared state
- Angular components should be small and focused — extract logic to services
