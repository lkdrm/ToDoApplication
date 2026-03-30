using ToDoApi.Models;

namespace ToDoApi.Services.Interfaces;

public interface IToDoService
{
    Task<ToDoItem> AddTaskAsync(CreateTaskRequest createTaskRequest);

    Task<List<ToDoItem>> GetAllTasksAsync();

    Task UpdateTaskAsync(ToDoItem task);

    Task<bool> DeleteTaskAsync(Guid taskId);

    Task<ToDoItem?> GetTaskByIdAsync(Guid id);
}
