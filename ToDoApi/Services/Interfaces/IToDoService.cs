using ToDoApi.Models;

namespace ToDoApi.Services.Interfaces;

public interface IToDoService
{
    ToDoItem AddTask(string title);

    Task<List<ToDoItem>> GetAllTasksAsync();
}
