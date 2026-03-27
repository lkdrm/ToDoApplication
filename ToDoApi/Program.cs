using ToDoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IToDoService, InMemoryToDoService>();

var app = builder.Build();

app.MapGet("/tasks", (IToDoService service) => 
{ 
    return service.GetAllTasks(); }
);
app.MapPost("/tasks", (string title, IToDoService service) => 
{ 
    return service.AddTask(title); 
});

app.Run();
