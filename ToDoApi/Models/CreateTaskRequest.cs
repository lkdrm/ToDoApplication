namespace ToDoApi.Models;

public record CreateTaskRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
}
