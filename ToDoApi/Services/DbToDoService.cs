using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

/// <summary>
/// EF Core–backed implementation of <see cref="IToDoService"/>.
/// Performs all CRUD operations against a SQLite database via <see cref="ToDoContext"/>.
/// </summary>
public class DbToDoService : IToDoService
{
    private readonly ToDoContext _toDoContext;

    /// <summary>
    /// Initialises the service with the EF Core database context.
    /// </summary>
    /// <param name="toDoContext">The scoped database context injected by the DI container.</param>
    public DbToDoService(ToDoContext toDoContext)
    {
        _toDoContext = toDoContext;
    }

    /// <inheritdoc/>
    public async Task<ToDoItem> AddTaskAsync(CreateTaskRequest createTaskRequest)
    {
        var newTask = new ToDoItem
        {
            Id = Guid.NewGuid(),
            Title = createTaskRequest.Title,
            IsCompleted = false,
            Description = createTaskRequest.Description,
            CreatedDate = createTaskRequest.DateTime,
            // Initialise RowVersion with a unique value to enable optimistic concurrency from the first save.
            RowVersion = Guid.NewGuid().ToByteArray(),
        };

        await _toDoContext.ToDoItems.AddAsync(newTask);
        await _toDoContext.SaveChangesAsync();

        return newTask;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        var task = await _toDoContext.ToDoItems.FindAsync(taskId);

        if (task == null)
        {
            return false;
        }

        _toDoContext.ToDoItems.Remove(task);
        await _toDoContext.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<List<ToDoItem>> GetAllTasksAsync() => await _toDoContext.ToDoItems.ToListAsync();

    /// <inheritdoc/>
    public async Task<ToDoItem> GetTaskByIdAsync(Guid id) => await _toDoContext.ToDoItems?.FirstOrDefaultAsync(task => task.Id == id);

    /// <inheritdoc/>
    public async Task UpdateTaskAsync(ToDoItem item)
    {
        _toDoContext.ToDoItems.Update(item);
        await _toDoContext.SaveChangesAsync();
    }
}
