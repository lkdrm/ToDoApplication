using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.Services;
using ToDoApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoContext>(options => options.UseInMemoryDatabase("ToDoList"));

builder.Services.AddHostedService<EmailWorker>();

builder.Services.AddScoped<IToDoService, DbToDoService>();
builder.Services.AddScoped<IScopedService, ScopedService>();
builder.Services.AddTransient<ITransientService, TransientService>();
builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();

app.MapGet("/tasks", async (IToDoService service, IAuditService audit) =>
{
    await audit.LogActivityAsync("User requested task list");
    return await service.GetAllTasksAsync();
}
);

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

app.MapPut("/tasks/{id}", async (Guid id, ToDoItem toDoItem, IToDoService service) =>
{
    var task = await service.GetTaskByIdAsync(id);
    if (task is null)
    {
        return Results.NotFound();
    }

    try
    {
        task.IsCompleted = toDoItem.IsCompleted;
        await service.UpdateTaskAsync(task);
        return Results.Ok(task);
    }
    catch (DbUpdateConcurrencyException)
    {
        return Results.Conflict("Update the page");
    }
});

app.MapPost("/tasks", async (CreateTaskRequest request, IToDoService service) =>
{
    var task = await service.AddTaskAsync(request);
    return Results.Created($"/tasks/{task.Id}", task);
});

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

app.UseStaticFiles();
await app.RunAsync();