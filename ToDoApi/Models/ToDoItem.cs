namespace ToDoApi.Models;

public record ToDoItem(Guid Id, string Title, bool IsCompleted);