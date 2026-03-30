# 📚 Bookboxd

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)
![Blazor WebAssembly](https://img.shields.io/badge/Blazor-WebAssembly-5C2D91?style=flat&logo=blazor)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat&logo=microsoft-sql-server)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

> A social platform for book lovers — inspired by Letterboxd — built with **.NET 9**.

Track your reading, write reviews, create lists, and follow other readers in a modern social experience.

---

## 🚀 Features

* **📖 Book Tracking:** Organize books into *Want to Read*, *Reading*, and *Read*.
* **⭐ Reviews & Ratings:** Write reviews with spoiler support and rate books from 1 to 5 stars.
* **🗂️ Custom Lists:** Create curated lists like "Best of 2024" with privacy control (Public, Private, or Friends only).
* **🧑‍🤝‍🧑 Social Feed:** Follow users and see their activity (reviews, ratings, lists) in real-time.
* **🔗 External Integrations:** Automatic book metadata and covers via the Open Library API.

---

## 🧰 Tech Stack

| Layer | Technology |
| :--- | :--- |
| **Backend** | C# / .NET 9 |
| **API** | ASP.NET Core Web API |
| **Frontend** | Blazor WebAssembly |
| **Database** | SQL Server |
| **ORM** | Entity Framework Core |
| **Auth** | ASP.NET Identity + JWT |
| **Architecture** | Clean Architecture + CQRS |

---

## 🏗️ Architecture

This project follows **Clean Architecture**, ensuring low coupling and clear separation of concerns:

```text
API ─────────▶ Application ─────────▶ Domain
 │                ▲
 │                │
 └────────▶ Infrastructure ────────┘
````

### 📦 Projects

  * **`Bookboxd.Domain`**: Core business rules, entities, and value objects.
  * **`Bookboxd.Application`**: Use cases (CQRS), DTOs, validations, and interfaces.
  * **`Bookboxd.Infrastructure`**: Database (EF Core), external services, and implementations.
  * **`Bookboxd.API`**: Controllers, middleware, and dependency injection.
  * **`Bookboxd.Web`**: Blazor WebAssembly client.

-----

## ⚡ Getting Started

### ✅ Prerequisites

  * [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
  * SQL Server (or LocalDB)
  * Visual Studio / Rider / VS Code

### ▶️ Run Locally

**1. Clone the repository**

```bash
git clone [https://github.com/torvigoes/BookBooks.git](https://github.com/torvigoes/BookBooks.git)
cd BookBooks
```

**2. Configure Database**
Edit `Bookboxd.API/appsettings.Development.json` to include your connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=Bookboxd;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**3. Apply Migrations**

```bash
dotnet ef database update --project Bookboxd.Infrastructure --startup-project Bookboxd.API
```

**4. Run the API**

```bash
cd Bookboxd.API
dotnet run
```

**5. Run the Web App**

```bash
cd ../Bookboxd.Web
dotnet run
```

-----

## 🧪 Future Improvements

  * 🧠 AI-powered recommendations
  * 📊 Reading statistics dashboard
  * 🔔 Notifications system

-----

## 🤝 Contributing

Contributions are welcome\! If you'd like to help improve Bookboxd:

```bash
git checkout -b feature/your-feature
git commit -m "feat: add new feature"
git push origin feature/your-feature
```

Then open a Pull Request 🚀

-----

## 📄 License

This project is licensed under the [MIT License](https://www.google.com/search?q=LICENSE).
