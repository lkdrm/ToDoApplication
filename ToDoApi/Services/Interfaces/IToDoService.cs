using ToDoApi.Models;

namespace ToDoApi.Services.Interfaces;

public interface IToDoService
{
    Task<ToDoItem> AddTaskAsync(string title);

    Task<List<ToDoItem>> GetAllTasksAsync();

    Task UpdateTaskAsync(ToDoItem task);

    Task<ToDoItem?> GetTaskByIdAsync(Guid id);
}
