using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

/// <summary>
/// Scoped service that logs user activity together with the current total task count.
/// </summary>
public class AuditService : IAuditService
{
    private readonly IToDoService _toDoService;
    private readonly ILogger<AuditService> _logger;

    /// <summary>
    /// Initialises the audit service with its required dependencies.
    /// </summary>
    /// <param name="toDoService">Used to fetch the current task count for each log entry.</param>
    /// <param name="logger">Logger that receives the formatted audit messages.</param>
    public AuditService(IToDoService toDoService, ILogger<AuditService> logger)
    {
        _toDoService = toDoService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task LogActivityAsync(string activity)
    {
        // Fetch the current task count so the audit entry contains useful context.
        var tasks = await _toDoService.GetAllTasksAsync();
        _logger.LogInformation($"[Audit] Service: {_toDoService.GetType().Name} | Activity: {activity} | Total tasks: {tasks.Count}");
    }
}
