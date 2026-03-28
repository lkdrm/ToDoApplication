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

    public async Task<ToDoItem> AddTaskAsync(string title)
    {
        var newTask = new ToDoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            IsCompleted = false
        };

        await _toDoContext.ToDoItems.AddAsync(newTask);
        await _toDoContext.SaveChangesAsync();

        return newTask;
    }

    public async Task<List<ToDoItem>> GetAllTasksAsync() => await _toDoContext.ToDoItems.ToListAsync();

    public async Task<ToDoItem> GetTaskByIdAsync(Guid id) => await _toDoContext.ToDoItems?.FirstOrDefaultAsync(task => task.Id == id);

    public async Task UpdateTaskAsync(ToDoItem item)
    {
        _toDoContext.ToDoItems.Update(item);
        await _toDoContext.SaveChangesAsync();
    }
}
