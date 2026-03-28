using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

public class TransientService : ITransientService
{
    public Guid ServiceId { get; init; } = Guid.NewGuid();
}
