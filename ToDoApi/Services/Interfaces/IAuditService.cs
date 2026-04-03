namespace ToDoApi.Services.Interfaces;

/// <summary>
/// Logs user activity alongside the current state of the task list.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Writes an audit log entry that includes the given <paramref name="activity"/> description
    /// and the total number of tasks currently in the store.
    /// </summary>
    /// <param name="activity">A human-readable description of the action being audited.</param>
    Task LogActivityAsync(string activity);
}
