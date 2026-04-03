using ToDoApi.Models;

namespace ToDoApi.Services.Interfaces;

/// <summary>
/// Defines CRUD operations for to-do tasks.
/// </summary>
public interface IToDoService
{
    /// <summary>
    /// Creates a new task from <paramref name="createTaskRequest"/> and persists it to the store.
    /// </summary>
    /// <param name="createTaskRequest">The DTO carrying the title, description, and creation time.</param>
    /// <returns>The newly created <see cref="ToDoItem"/> including its generated ID.</returns>
    Task<ToDoItem> AddTaskAsync(CreateTaskRequest createTaskRequest);

    /// <summary>
    /// Returns all tasks currently in the data store.
    /// </summary>
    /// <returns>A list of all <see cref="ToDoItem"/> records.</returns>
    Task<List<ToDoItem>> GetAllTasksAsync();

    /// <summary>
    /// Persists any changes made to an existing <paramref name="task"/>.
    /// </summary>
    /// <param name="task">The task with updated property values.</param>
    Task UpdateTaskAsync(ToDoItem task);

    /// <summary>
    /// Deletes the task with the specified <paramref name="taskId"/>.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task to remove.</param>
    /// <returns><c>true</c> if the task was found and deleted; <c>false</c> if it did not exist.</returns>
    Task<bool> DeleteTaskAsync(Guid taskId);

    /// <summary>
    /// Finds the task with the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The unique identifier to search for.</param>
    /// <returns>The matching <see cref="ToDoItem"/>, or <c>null</c> if not found.</returns>
    Task<ToDoItem?> GetTaskByIdAsync(Guid id);
}
