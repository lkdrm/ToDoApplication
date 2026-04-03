using Microsoft.EntityFrameworkCore;
using ToDoApi.Models;

namespace ToDoApi.Data;

/// <summary>
/// Represents the database context for the To-Do application,
/// providing access to the underlying data store via Entity Framework Core.
/// </summary>
public class ToDoContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoContext"/> class.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
    { }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="ToDoItem"/> entities.
    /// </summary>
    public DbSet<ToDoItem> ToDoItems { get; set; }

    /// <summary>
    /// Configures the entity model, setting <see cref="ToDoItem.RowVersion"/>
    /// as a concurrency token to handle optimistic concurrency conflicts.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDoItem>()
            .Property(t => t.RowVersion)
            .IsConcurrencyToken();
    }
}
