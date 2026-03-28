using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

public class ScopedService : IScopedService
{
    public Guid ServiceId { get; init; } = Guid.NewGuid();
}
