using System.ComponentModel.DataAnnotations;

namespace ToDoApi.Models;

/// <summary>
/// Represents a single to-do task stored in the database.
/// </summary>
public record ToDoItem
{
    /// <summary>
    /// Unique identifier of the task.
    /// Uses <c>init</c> because the ID is assigned once at creation and never changes.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Short, human-readable title of the task.
    /// Uses <c>set</c> because the title can be updated via the PUT endpoint.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Whether the task has been marked as completed.
    /// Uses <c>set</c> because it is toggled by the client via the PUT endpoint.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Detailed description of what the task involves.
    /// Uses <c>set</c> because the description can be updated via the PUT endpoint.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The date and time when the task was originally created.
    /// Uses <c>init</c> because the creation date is set once and must never change.
    /// </summary>
    public DateTime CreatedDate { get; init; }

    /// <summary>
    /// Optimistic-concurrency token managed by EF Core.
    /// EF Core compares this value before every UPDATE; a mismatch throws
    /// <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException"/>.
    /// Uses <c>set</c> so EF Core can update the value when it materialises the entity.
    /// </summary>
    public byte[] RowVersion { get; set; }
}