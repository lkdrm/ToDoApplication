using ToDoApi.Models;

namespace ToDoApi.Services;

public interface IToDoService
{
    ToDoItem AddTask(string title);

    List<ToDoItem> GetAllTasks();
}
