using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

public class AuditService : IAuditService
{
    private readonly IToDoService _toDoService;

    public AuditService(IToDoService toDoService)
    {
        _toDoService = toDoService;
    }

    public async Task LogActivityAsync(string activity) => Console.WriteLine($"[Audit] Service: {_toDoService.GetType().Name} | Activity: {activity} | Total tasks: {_toDoService.GetAllTasksAsync().Result.Count}");
}
