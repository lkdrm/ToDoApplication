using System.ComponentModel.DataAnnotations;

namespace ToDoApi.Models;

public record ToDoItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public byte[] RowVersion { get; set; }
}