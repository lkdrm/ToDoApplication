namespace ToDoApi.Models;

/// <summary>
/// Data Transfer Object received from the client when creating a new task.
/// Properties use <c>init</c> because the object is only populated during JSON
/// deserialization and must not be mutated afterwards.
/// </summary>
public record CreateTaskRequest
{
    /// <summary>
    /// Short, human-readable title of the new task.
    /// Must not be empty; validated on the frontend before the request is sent.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Detailed description of what the task involves.
    /// Must not be empty; validated on the frontend before the request is sent.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Creation timestamp provided by the client (browser local time).
    /// Stored as <see cref="ToDoItem.CreatedDate"/> after the task is persisted.
    /// </summary>
    public DateTime DateTime { get; init; }
}
