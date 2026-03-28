using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

public class EmailWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(IServiceScopeFactory scopeFactory, ILogger<EmailWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmailWorker started.");

        using PeriodicTimer periodicTimer = new(TimeSpan.FromMinutes(1));

        try
        {
            while (await periodicTimer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var toDoService = scope.ServiceProvider.GetRequiredService<IToDoService>();

                    var getAllTasks = await toDoService.GetAllTasksAsync();
                    var uncompletedTasks = getAllTasks.Count(task => !task.IsCompleted);
                    if (uncompletedTasks > 0)
                    {
                        _logger.LogInformation($"[NOTIFICATION] Notification: U have {uncompletedTasks} unresolved tasks.");
                    }
                    else
                    {
                        _logger.LogInformation("No pending tasks to process.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during task processing: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("EmailWorker is shutting down...");
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Fatal error in background service: {ex.Message}");
        }
    }
}
