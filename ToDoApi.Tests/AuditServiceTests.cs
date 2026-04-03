using Microsoft.Extensions.Logging;
using Moq;
using ToDoApi.Models;
using ToDoApi.Services;
using ToDoApi.Services.Interfaces;

namespace ToDoApi.Tests;

public class AuditServiceTests
{
    private readonly Mock<IToDoService> _toDoServiceMock = new();
    private readonly Mock<ILogger<AuditService>> _loggerMock = new();
    private readonly AuditService _service;

    public AuditServiceTests()
    {
        _service = new AuditService(_toDoServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task LogActivityAsync_CallsGetAllTasksAsync()
    {
        _toDoServiceMock.Setup(s => s.GetAllTasksAsync()).ReturnsAsync([]);

        await _service.LogActivityAsync("Test activity");

        _toDoServiceMock.Verify(s => s.GetAllTasksAsync(), Times.Once);
    }

    [Fact]
    public async Task LogActivityAsync_LogsInformation_WithActivityAndTaskCount()
    {
        var tasks = new List<ToDoItem>
        {
            new() { Id = Guid.NewGuid(), Title = "T", Description = "D", RowVersion = [] },
            new() { Id = Guid.NewGuid(), Title = "T2", Description = "D2", RowVersion = [] }
        };
        _toDoServiceMock.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(tasks);

        await _service.LogActivityAsync("Test activity");

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Test activity") && v.ToString()!.Contains("2")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogActivityAsync_LogsInformation_WhenNoTasks()
    {
        _toDoServiceMock.Setup(s => s.GetAllTasksAsync()).ReturnsAsync([]);

        await _service.LogActivityAsync("Empty list check");

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Empty list check") && v.ToString()!.Contains("0")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
