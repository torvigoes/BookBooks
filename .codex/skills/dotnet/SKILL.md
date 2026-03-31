# BookBooks — Codex Agent Skill

> This file defines rules, patterns, and conventions for an autonomous Codex agent
> working on the BookBooks repository. Follow every section before generating or editing any file.

---

## 1. Project Snapshot

| Item | Value |
|---|---|
| Solution | `BookBooks.sln` |
| Runtime | .NET 9 / C# 13 |
| Frontend | Blazor WebAssembly (`BookBooks.Web`) |
| Database | SQL Server — EF Core 9, Code First |
| Auth | ASP.NET Core Identity + JWT Bearer |
| CQRS | MediatR + FluentValidation |
| API style | RESTful JSON, RFC 7807 ProblemDetails errors |

---

## 2. Solution Map

```
BookBooks.sln
├── BookBooks.Domain          → Entities, enums, domain interfaces, Result<T>
├── BookBooks.Application     → Commands/Queries (CQRS), validators, handlers, DTOs
├── BookBooks.Infrastructure  → EF Core, repositories, JWT provider
├── BookBooks.API             → Controllers, middleware, DI wiring, Swagger
└── BookBooks.Web             → Blazor WebAssembly client (in progress)
```

### Dependency Rules — NEVER violate these

- **Domain** → zero external dependencies, no NuGet packages
- **Application** → depends only on Domain
- **Infrastructure** → depends on Domain + Application
- **API** → depends on Application + Infrastructure
- **Web** → depends only on API's HTTP contract

---

## 3. What Is Already Implemented

Before generating anything, check whether it already exists:

| Area | Status |
|---|---|
| `POST /api/auth/register` | ✅ Done |
| `POST /api/auth/login` | ✅ Done |
| `POST /api/books` | ✅ Done |
| `GET /api/books/{id}` | ✅ Done |
| `POST /api/books/{bookId}/reviews` | ✅ Done |
| `PUT /api/reviews/{reviewId}` | ✅ Done |
| `DELETE /api/reviews/{reviewId}` | ✅ Done |
| `POST /api/reviews/{reviewId}/like` | ✅ Done |
| `GET /api/books/{bookId}/reviews` | ✅ Done |
| EF Core migrations (initial) | ✅ Done |
| Reading status endpoints | ❌ Not implemented |
| Lists endpoints | ❌ Not implemented |
| Follow/feed endpoints | ❌ Not implemented |
| Automated tests | ❌ Not implemented |
| Blazor UI (beyond scaffold) | ❌ Not implemented |

---

## 4. Domain Layer Rules (`BookBooks.Domain`)

### Entity conventions

```csharp
// ✅ CORRECT — enforces invariants, EF Core constructor is explicit
public class Book
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Isbn { get; private set; }
    public int Year { get; private set; }

    public Book(string title, string author, string isbn, int year)
    {
        Title = title;
        Author = author;
        Isbn = isbn;
        Year = year;
    }

    /// <summary>
    /// Protected constructor for EF Core use only.
    /// Properties will be hydrated via column mapping.
    /// </summary>
    protected Book()
    {
        Title = null!;
        Author = null!;
        Isbn = null!;
    }
}
```

### Rules for every entity

- All properties → `private set` or `init` only
- Public constructor → assigns all required fields, enforces invariants
- Protected parameterless constructor → for EF Core only, string props = `null!`, with XML comment
- Never use `= string.Empty` or `= null!` at the property declaration level
- Never use public setters on domain entities
- `Id` → always `Guid.NewGuid().ToString()`, initialized at declaration

### Existing entities — do NOT recreate

`AppUser`, `Book`, `Review`, `BookList`, `BookListItem`, `UserFollow`, `ReviewLike`, `ReadingStatus`

### Enums

```csharp
public enum ReadingStatusType { WantToRead, CurrentlyReading, Read, Abandoned }
public enum ListVisibility    { Public, Private, FriendsOnly }
```

### Domain interfaces (must live in Domain, implemented in Infrastructure)

```csharp
IBookRepository       // CRUD + search
IReviewRepository     // by book, by user, feed
IBookListRepository   // user lists
IUserRepository       // follow/unfollow, search
IUnitOfWork           // wraps DbContext.SaveChangesAsync()
```

---

## 5. Application Layer Rules (`BookBooks.Application`)

### Feature folder structure — always follow this layout

```
Features/
└── {Feature}/
    ├── Commands/
    │   ├── {Verb}{Noun}Command.cs
    │   ├── {Verb}{Noun}CommandHandler.cs
    │   └── {Verb}{Noun}CommandValidator.cs
    ├── Queries/
    │   ├── {Verb}{Noun}Query.cs
    │   ├── {Verb}{Noun}QueryHandler.cs
    │   └── {Verb}{Noun}QueryValidator.cs   (only if query has parameters)
    └── DTOs/
        └── {Noun}Dto.cs
```

### Command/Query template

```csharp
// Command
public record CreateBookCommand(string Title, string Author, string Isbn, int Year)
    : IRequest<Result<BookDto>>;

// Handler
public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    private readonly IBookRepository _books;
    private readonly IUnitOfWork _uow;

    public CreateBookCommandHandler(IBookRepository books, IUnitOfWork uow)
    {
        _books = books;
        _uow = uow;
    }

    public async Task<Result<BookDto>> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        // implementation
    }
}

// Validator
public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Isbn).NotEmpty().Length(10, 13);
        RuleFor(x => x.Year).InclusiveBetween(1000, DateTime.UtcNow.Year);
    }
}
```

### Result<T> pattern — always use, never throw from Application

```csharp
// Success
return Result<BookDto>.Success(dto);

// Failure
return Result<BookDto>.Failure("Book not found.");
return Result<BookDto>.Failure("ISBN already exists.");
```

### DTOs — always use records

```csharp
/// <summary>Full book representation returned to API consumers.</summary>
public record BookDto(
    string Id,
    string Title,
    string Author,
    string Isbn,
    int Year,
    string? CoverImageUrl,
    double AverageRating,
    int ReviewCount);
```

### Mapping — Mapster, one profile per feature folder

```csharp
public sealed class BookMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Book, BookDto>();
    }
}
```

---

## 6. Infrastructure Layer Rules (`BookBooks.Infrastructure`)

### Repository template

```csharp
public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _db;

    public BookRepository(AppDbContext db) => _db = db;

    public async Task<Book?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _db.Books.FindAsync([id], ct);

    public async Task AddAsync(Book book, CancellationToken ct = default)
        => await _db.Books.AddAsync(book, ct);
}
```

### EF Core configuration — Fluent API only, no data annotations on entities

```csharp
public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(500);
        builder.Property(b => b.Isbn).IsRequired().HasMaxLength(13);
        builder.HasIndex(b => b.Isbn).IsUnique();
    }
}
```

### Composite keys

```csharp
// UserFollow
entity.HasKey(f => new { f.FollowerId, f.FollowedId });

// ReviewLike
entity.HasKey(l => new { l.UserId, l.ReviewId });

// BookListItem
entity.HasKey(i => new { i.BookListId, i.BookId });
```

---

## 7. API Layer Rules (`BookBooks.API`)

### Controller template

```csharp
[ApiController]
[Route("api/[controller]")]
public sealed class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBookByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }
}
```

### Response conventions — always follow

| Outcome | HTTP Status |
|---|---|
| Success with body | `200 OK` |
| Resource created | `201 Created` + `Location` header |
| Validation error | `400 Bad Request` + FluentValidation error list |
| Not found | `404 Not Found` + ProblemDetails |
| Unauthorized | `401` |
| Forbidden | `403` |
| All list endpoints | Paginated: `?page=1&pageSize=20` |

### Business rules — enforce at service and DB level

1. One review per user per book (unique index + Application guard)
2. A user cannot follow themselves
3. Deleting a review cascades to its likes
4. `Book.AverageRating` recalculated after every review insert/update/delete
5. `ListVisibility.Private` lists are never returned to other users
6. `ReadingStatus` is always upserted (one record per user+book)
7. ISBN unique in DB
8. Review rating: 1–5 / content: 10–5000 characters

---

## 8. Naming Conventions

### C#

| Element | Convention | Example |
|---|---|---|
| Entities | Singular PascalCase | `Book`, `BookList` |
| DTOs | Suffix `Dto` / `Request` / `Response` | `BookDto`, `CreateBookRequest` |
| Interfaces | Prefix `I` | `IBookRepository` |
| Commands/Queries | Verb + Noun + suffix | `CreateBookCommand`, `GetBookByIdQuery` |
| Handlers | Command/Query name + `Handler` | `CreateBookCommandHandler` |
| Validators | Command/Query name + `Validator` | `CreateBookCommandValidator` |
| Controllers | Plural resource | `BooksController` |

### Blazor / TypeScript

| Element | Convention |
|---|---|
| Components | PascalCase file, kebab-case selector |
| Services | PascalCase class |
| Interfaces/Models | PascalCase, no `I` prefix |

---

## 9. Code Style Checklist

Before finalizing any generated file, verify:

- [ ] `async`/`await` + `CancellationToken` on every I/O method
- [ ] `sealed` on all concrete Application and Infrastructure classes
- [ ] Records used for all DTOs
- [ ] No `throw` in Application layer — use `Result<T>`
- [ ] No data annotations on Domain entities — Fluent API only
- [ ] No public setters on Domain entities
- [ ] XML doc comments on all public interfaces and DTOs
- [ ] FluentValidation validator exists for every Command/Query with parameters
- [ ] Mapster profile exists for every new entity ↔ DTO mapping
- [ ] New EF entity registered as `DbSet<T>` in `AppDbContext`
- [ ] New EF entity has an `IEntityTypeConfiguration<T>` in `Configurations/`

---

## 10. Development Backlog (Priority Order)

Work items not yet implemented — tackle in this order:

**Priority 1**
- Automated tests: Domain unit tests, Application unit tests, API integration tests
- Blazor: auth flow (register + login), book search, book detail, review submission
- Move JWT secret out of tracked config

**Priority 2**
- Lists feature: `CreateList`, `AddBookToList`, `RemoveBookFromList`, `DeleteList`, `GetListById`, `GetUserLists` + endpoints
- Reading status feature: upsert + retrieval + Blazor UI
- Follow/feed feature: `FollowUser`, `UnfollowUser`, `GetUserFeed`, `GetFollowers`, `GetFollowing` + endpoints

**Priority 3**
- Notifications
- Reading analytics dashboard
- External book metadata sync (Open Library API integration)

---

## 11. External Integrations

| Service | Base URL | Key Class |
|---|---|---|
| Open Library API | `https://openlibrary.org` | `OpenLibraryService` |
| Book covers | `https://covers.openlibrary.org/b/isbn/{ISBN}-L.jpg` | `CoverImageService` |

---

## 12. Agent Workflow

When given a task, always follow this sequence:

1. **Read this file** before touching any code
2. **Check the "Already Implemented" table** (§3) — do not recreate existing work
3. **Identify the layer** — Domain / Application / Infrastructure / API / Web
4. **Check dependency rules** (§2) — never import across forbidden boundaries
5. **Generate files** following the templates in §4–7
6. **Run the checklist** in §9 before finishing
7. **Do not modify** unrelated files — surgical edits only
