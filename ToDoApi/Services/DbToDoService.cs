using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

public class DbToDoService : IToDoService
{
    private readonly ToDoContext _toDoContext;

    public DbToDoService(ToDoContext toDoContext)
    {
        _toDoContext = toDoContext;
    }

    public async Task<ToDoItem> AddTaskAsync(CreateTaskRequest createTaskRequest)
    {
        var newTask = new ToDoItem
        {
            Id = Guid.NewGuid(),
            Title = createTaskRequest.Title,
            IsCompleted = false,
            Description = createTaskRequest.Description,
            CreatedDate = createTaskRequest.DateTime,
            RowVersion = Guid.NewGuid().ToByteArray(),
        };

        await _toDoContext.ToDoItems.AddAsync(newTask);
        await _toDoContext.SaveChangesAsync();

        return newTask;
    }

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

    public async Task<List<ToDoItem>> GetAllTasksAsync() => await _toDoContext.ToDoItems.ToListAsync();

    public async Task<ToDoItem> GetTaskByIdAsync(Guid id) => await _toDoContext.ToDoItems?.FirstOrDefaultAsync(task => task.Id == id);

    public async Task UpdateTaskAsync(ToDoItem item)
    {
        _toDoContext.ToDoItems.Update(item);
        await _toDoContext.SaveChangesAsync();
    }
}
