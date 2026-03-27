using ToDoApi.Models;

namespace ToDoApi.Services;

public class InMemoryToDoService : IToDoService
{
    private readonly Dictionary<Guid, ToDoItem> _tasks = new();

    public ToDoItem AddTask(string title)
    {
        var genId = Guid.NewGuid();
        var newTask = new ToDoItem(genId, title, false);
        _tasks.Add(genId, newTask);
        return newTask;
    }

    public List<ToDoItem> GetAllTasks() => _tasks.Values.ToList();
}
