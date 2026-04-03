using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

/// <summary>
/// Singleton background service that periodically checks for uncompleted tasks
/// and logs a reminder notification — simulating an email or push alert.
/// </summary>
public class EmailWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailWorker> _logger;

    /// <summary>
    /// Initialises the worker with a scope factory and a logger.
    /// </summary>
    /// <param name="scopeFactory">Used to create a new DI scope on every timer tick.</param>
    /// <param name="logger">Logger for startup, notification, and error messages.</param>
    public EmailWorker(IServiceScopeFactory scopeFactory, ILogger<EmailWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Runs the notification loop until the application shuts down.
    /// Fires every minute via <see cref="PeriodicTimer"/>; exits cleanly when
    /// <paramref name="stoppingToken"/> is cancelled.
    /// </summary>
    /// <param name="stoppingToken">Signals when the host is requesting a graceful stop.</param>
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
                    // Create a short-lived scope so the scoped IToDoService
                    // (and its underlying DbContext) is properly disposed after each tick.
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
                    // Log and continue so a single failed tick does not stop the worker.
                    _logger.LogError($"Error during task processing: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the host requests a graceful shutdown.
            _logger.LogInformation("EmailWorker is shutting down...");
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Fatal error in background service: {ex.Message}");
        }
    }
}
