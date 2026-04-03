# ToDoApplication

A full-stack To-Do web application built with **ASP.NET Core Minimal API** (.NET 10) and a **TypeScript** frontend. Tasks are persisted to a **SQLite** database via Entity Framework Core, and a background worker periodically notifies about uncompleted tasks.

---

## What is it?

ToDoApplication lets you create, view, update, filter, and delete personal tasks through a clean browser UI. Each task has a title, description, creation date, and a completion state. The app handles concurrent edits safely using optimistic concurrency (EF Core `RowVersion` token) and logs every significant user action through a built-in audit service.

---

## How does it work?

### Backend — REST API

The backend exposes five HTTP endpoints defined in `Program.cs`:

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/tasks` | Returns all tasks; writes an audit log entry |
| `POST` | `/tasks` | Creates a new task; returns `201 Created` |
| `PUT` | `/tasks/{id}` | Updates title, description, and completion state; returns `409 Conflict` on concurrent edit |
| `DELETE` | `/tasks/{id}` | Deletes a task by ID; returns `404` if not found |
| `GET` | `/lifetimes` | Diagnostic endpoint that demonstrates DI lifetime behaviour (scoped vs transient) |

### Frontend — Static SPA

The frontend (`wwwroot/index.html` + `wwwroot/app.js`) is a single-page app served as static files. It communicates with the REST API via `fetch` and supports:

- Adding a task (title + description, both required)
- Marking a task as completed
- Editing a task inline
- Deleting a task
- Filtering tasks by **All / Active / Completed**
- Sorting tasks by creation date (newest first)

The source is written in TypeScript (`Scripts/app.ts`) and compiled to `wwwroot/app.js`.

### Background Service — EmailWorker

`EmailWorker` is a singleton `BackgroundService` that ticks every **60 seconds**. On each tick it creates a fresh DI scope, queries the database for uncompleted tasks, and logs a reminder notification. Errors on individual ticks are caught and logged so the worker never stops unexpectedly.

### Audit Service

`AuditService` is injected into the `GET /tasks` handler. Every time the task list is requested it fetches the current task count and writes a structured log line:

```
[Audit] Service: DbToDoService | Activity: User requested task list | Total tasks: 5
```

### Dependency Injection lifetimes

The project intentionally registers services with different lifetimes to demonstrate DI behaviour:

| Service | Lifetime | Purpose |
|---------|----------|---------|
| `DbToDoService` | Scoped | One instance per HTTP request |
| `AuditService` | Scoped | One instance per HTTP request |
| `ScopedService` | Scoped | Lifetime demo — same `ServiceId` within a request |
| `TransientService` | Transient | Lifetime demo — new `ServiceId` on every injection |
| `EmailWorker` | Singleton (hosted) | Long-running background worker |

---

## Technologies used

| Layer | Technology |
|-------|------------|
| Runtime | .NET 10 |
| Web framework | ASP.NET Core Minimal API |
| ORM | Entity Framework Core 10 |
| Database | SQLite |
| Frontend language | TypeScript |
| Frontend markup/styles | HTML5, CSS3 |
| Unit testing | xUnit |
| Mocking | Moq |
| Test DB | EF Core InMemory |
| Code coverage | coverlet |

---

## Project structure

```
ToDoApplication/
├── ToDoApi/                            # Main web application
│   ├── Data/
│   │   └── ToDoContext.cs              # EF Core DbContext with RowVersion concurrency token
│   ├── Migrations/                     # EF Core database migrations
│   ├── Models/
│   │   ├── ToDoItem.cs                 # Entity — persisted task record
│   │   └── CreateTaskRequest.cs        # DTO for POST /tasks
│   ├── Scripts/
│   │   └── app.ts                      # TypeScript frontend source
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   ├── IAuditService.cs
│   │   │   ├── IScopedService.cs
│   │   │   ├── IToDoService.cs
│   │   │   └── ITransientService.cs
│   │   ├── AuditService.cs             # Logs user activity with task count
│   │   ├── DbToDoService.cs            # EF Core CRUD implementation
│   │   ├── EmailWorker.cs              # Background notification worker
│   │   ├── ScopedService.cs            # DI lifetime demo — scoped
│   │   └── TransientService.cs         # DI lifetime demo — transient
│   ├── wwwroot/
│   │   ├── index.html                  # App shell
│   │   ├── app.js                      # Compiled TypeScript
│   │   └── style.css                   # Styles
│   ├── Program.cs                      # Entry point — DI setup and endpoint mapping
│   └── ToDoApi.csproj
│
└── ToDoApi.Tests/                      # Unit test project
    ├── AuditServiceTests.cs            # Tests for AuditService (Moq)
    ├── DbToDoServiceTests.cs           # Tests for DbToDoService (InMemory DB)
    ├── ScopedServiceTests.cs           # Tests for ScopedService
    ├── TransientServiceTests.cs        # Tests for TransientService
    └── ToDoApi.Tests.csproj
```

---

## Getting started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the application
```bash
dotnet run --project ToDoApi
```
The app starts on `https://localhost:5001` (or the port shown in the console). Open it in a browser to use the UI.

### Run the database migration
EF Core applies migrations automatically on first run. To apply manually:
```bash
dotnet ef database update --project ToDoApi
```

### Run the tests
```bash
dotnet test
```
