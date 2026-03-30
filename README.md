# BookBooks ??

BookBooks is a book-tracking social platform inspired by Letterboxd, built with .NET 9. It allows users to catalog books they have read, write reviews with ratings, organize books into custom lists, and follow other users to see their reading activity.

## ?? Features

- **Book Catalog:** Discover and keep track of books you want to read, are currently reading, or have already read.
- **Reviews & Ratings:** Share your thoughts on books with rich text reviews (including spoiler tags) and a 1-5 star rating system.
- **Custom Lists:** Curate your own custom book lists (e.g., "Favorites of 2024", "Sci-Fi Must Reads") with customizable visibility settings (Public, Private, Friends Only).
- **Social Feed:** Follow other readers and stay up to date with their latest reads, reviews, and lists in a dynamic activity feed.
- **Seamless Integrations:** Automated fetching of book metadata and cover images using the Open Library API.

## ?? Tech Stack

- **Backend:** C# / .NET 9
- **Framework:** ASP.NET Core Web API
- **Frontend / Client:** Blazor WebAssembly (BookBooks.Web)
- **Database:** SQL Server
- **ORM:** Entity Framework Core (Code First)
- **Authentication:** ASP.NET Core Identity + JWT Bearer Tokens
- **Architecture:** Clean Architecture & CQRS Pattern

## ??? Project Structure

The solution follows Clean Architecture principles, ensuring a separation of concerns and maintaining low coupling:

- `BookBooks.Domain` - Core entities, enums, domain interfaces, and domain events.
- `BookBooks.Application` - Use cases (Commands/Queries), DTOs, validators, and mapping logic.
- `BookBooks.Infrastructure` - EF Core DbContext, repository implementations, Auth configurations, and external integrations (Open Library).
- `BookBooks.API` - ASP.NET Core controllers, middleware, and Dependency Injection wiring.
- `BookBooks.Web` - The client application (Blazor WebAssembly).

## ??? Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (or SQL Server Express / LocalDB for local development)
- Visual Studio 2022, JetBrains Rider, or VS Code

### Run Locally

1. **Clone the repository:**
   ```bash
   git clone https://github.com/torvigoes/BookBooks.git
   cd BookBooks
   ```

2. **Configure the Database Connection:**
   Open `BookBooks.API/appsettings.Development.json` and configure your `DefaultConnection` string.

3. **Apply Entity Framework Migrations:**
   Navigate into the `BookBooks.API` directory (or use the Package Manager Console) and run:
   ```bash
   dotnet ef database update --project ../BookBooks.Infrastructure --startup-project .
   ```

4. **Run the API:**
   ```bash
   cd BookBooks.API
   dotnet run
   ```
   The API will typically be exposed at `https://localhost:5001` or `http://localhost:5000` (check `Properties/launchSettings.json`).

5. **Run the Blazor Application:**
   Open a new terminal window:
   ```bash
   cd BookBooks.Web
   dotnet run
   ```

## ?? Contributing

Contributions are welcome! If you have suggestions or want to add a feature:
1. Fork the project.
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`).
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`).
4. Push to the Branch (`git push origin feature/AmazingFeature`).
5. Open a Pull Request.

## ?? License

This project is licensed under the MIT License.
