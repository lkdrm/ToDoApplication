using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.Services;
using ToDoApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
// SQLite file is created next to the binary; EF Core manages the schema via Migrations.
builder.Services.AddDbContext<ToDoContext>(options => options.UseSqlite("Data Source=todoList.db"));

// --- Background services ---
// EmailWorker is a singleton; it creates its own DI scope per timer tick
// to safely resolve scoped services such as IToDoService.
builder.Services.AddHostedService<EmailWorker>();

// --- Application services ---
builder.Services.AddScoped<IToDoService, DbToDoService>();    // One instance per HTTP request.
builder.Services.AddScoped<IScopedService, ScopedService>();  // Lifetime demo — scoped.
builder.Services.AddTransient<ITransientService, TransientService>(); // Lifetime demo — transient.
builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();

// GET /tasks — returns all tasks and writes an audit log entry.
app.MapGet("/tasks", async (IToDoService service, IAuditService audit) =>
{
    await audit.LogActivityAsync("User requested task list");
    return await service.GetAllTasksAsync();
}
);

// GET /lifetimes — diagnostic endpoint that shows DI lifetime behaviour.
// Scoped instances share the same ServiceId within one request; transient ones differ.
app.MapGet("/lifetimes", (ITransientService transient, IScopedService scopedService, ITransientService transient2, IScopedService scopedService2) =>
{
    return new
    {
        TransientService = transient.ServiceId,
        ScopedService = scopedService.ServiceId,
        TransientService2 = transient2.ServiceId,
        ScopedService2 = scopedService2.ServiceId
    };
});

// PUT /tasks/{id} — updates title, description, and completion state of an existing task.
// Returns 409 Conflict when another client modified the same task concurrently (RowVersion mismatch).
app.MapPut("/tasks/{id}", async (Guid id, ToDoItem toDoItem, IToDoService service) =>
{
    var task = await service.GetTaskByIdAsync(id);
    if (task is null)
    {
        return Results.NotFound();
    }

    try
    {
        task.Title = toDoItem.Title;
        task.Description = toDoItem.Description;
        task.IsCompleted = toDoItem.IsCompleted;
        await service.UpdateTaskAsync(task);
        return Results.Ok(task);
    }
    catch (DbUpdateConcurrencyException)
    {
        // The client's RowVersion no longer matches the stored value — ask them to refresh.
        return Results.Conflict("Update the page");
    }
});

// POST /tasks — creates a new task and returns 201 Created with the resource location.
app.MapPost("/tasks", async (CreateTaskRequest request, IToDoService service) =>
{
    var task = await service.AddTaskAsync(request);
    return Results.Created($"/tasks/{task.Id}", task);
});

// DELETE /tasks/{id} — removes the task; returns 404 if it does not exist.
app.MapDelete("/tasks/{id}", async (Guid id, IToDoService service) =>
{
    var isDeleted = await service.DeleteTaskAsync(id);
    if (isDeleted)
    {
        return Results.Ok();
    }
    else
    {
        return Results.NotFound();
    }
});

// Serve the static frontend (wwwroot/index.html and its assets).
app.UseDefaultFiles();
app.UseStaticFiles();
await app.RunAsync();