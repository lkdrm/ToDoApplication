using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.Services;

namespace ToDoApi.Tests;

public class DbToDoServiceTests : IDisposable
{
    private readonly ToDoContext _context;
    private readonly DbToDoService _service;

    public DbToDoServiceTests()
    {
        var options = new DbContextOptionsBuilder<ToDoContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ToDoContext(options);
        _service = new DbToDoService(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddTaskAsync_ReturnsTaskWithCorrectProperties()
    {
        var request = new CreateTaskRequest
        {
            Title = "Test Task",
            Description = "Test Description",
            DateTime = new DateTime(2024, 1, 1)
        };

        var result = await _service.AddTaskAsync(request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(new DateTime(2024, 1, 1), result.CreatedDate);
        Assert.False(result.IsCompleted);
        Assert.NotNull(result.RowVersion);
    }

    [Fact]
    public async Task AddTaskAsync_PersistsTaskToDatabase()
    {
        var request = new CreateTaskRequest { Title = "Task", Description = "Desc", DateTime = DateTime.UtcNow };

        var result = await _service.AddTaskAsync(request);

        var stored = await _context.ToDoItems.FindAsync(result.Id);
        Assert.NotNull(stored);
    }

    [Fact]
    public async Task GetAllTasksAsync_ReturnsAllTasks()
    {
        await _service.AddTaskAsync(new CreateTaskRequest { Title = "A", Description = "D1", DateTime = DateTime.UtcNow });
        await _service.AddTaskAsync(new CreateTaskRequest { Title = "B", Description = "D2", DateTime = DateTime.UtcNow });

        var result = await _service.GetAllTasksAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllTasksAsync_ReturnsEmptyList_WhenNoTasks()
    {
        var result = await _service.GetAllTasksAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsTask_WhenExists()
    {
        var added = await _service.AddTaskAsync(new CreateTaskRequest { Title = "T", Description = "D", DateTime = DateTime.UtcNow });

        var result = await _service.GetTaskByIdAsync(added.Id);

        Assert.NotNull(result);
        Assert.Equal(added.Id, result.Id);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _service.GetTaskByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_ReturnsTrue_WhenTaskExists()
    {
        var added = await _service.AddTaskAsync(new CreateTaskRequest { Title = "T", Description = "D", DateTime = DateTime.UtcNow });

        var result = await _service.DeleteTaskAsync(added.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_RemovesTaskFromDatabase()
    {
        var added = await _service.AddTaskAsync(new CreateTaskRequest { Title = "T", Description = "D", DateTime = DateTime.UtcNow });

        await _service.DeleteTaskAsync(added.Id);

        var stored = await _context.ToDoItems.FindAsync(added.Id);
        Assert.Null(stored);
    }

    [Fact]
    public async Task DeleteTaskAsync_ReturnsFalse_WhenTaskNotFound()
    {
        var result = await _service.DeleteTaskAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTaskAsync_PersistsChanges()
    {
        var added = await _service.AddTaskAsync(new CreateTaskRequest { Title = "Original", Description = "Desc", DateTime = DateTime.UtcNow });
        added.Title = "Updated";
        added.IsCompleted = true;

        await _service.UpdateTaskAsync(added);

        var stored = await _context.ToDoItems.FindAsync(added.Id);
        Assert.Equal("Updated", stored!.Title);
        Assert.True(stored.IsCompleted);
    }
}
