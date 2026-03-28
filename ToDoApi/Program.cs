using ToDoApi.Services;
using ToDoApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IToDoService, InMemoryToDoService>();
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
app.MapPost("/tasks", (string title, IToDoService service) =>
{
    return service.AddTask(title);
});

app.Run();
