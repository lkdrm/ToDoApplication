using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

public class AuditService : IAuditService
{
    private readonly IToDoService _toDoService;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IToDoService toDoService, ILogger<AuditService> logger)
    {
        _toDoService = toDoService;
        _logger = logger;
    }

    public async Task LogActivityAsync(string activity)
    {
        var tasks = await _toDoService.GetAllTasksAsync();
        _logger.LogInformation($"[Audit] Service: {_toDoService.GetType().Name} | Activity: {activity} | Total tasks: {tasks.Count}");
    }
}
