using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

/// <summary>
/// Scoped implementation of <see cref="IScopedService"/>.
/// The same instance — and therefore the same <see cref="ServiceId"/> — is reused
/// for every injection point within a single HTTP request.
/// </summary>
public class ScopedService : IScopedService
{
    /// <inheritdoc/>
    public Guid ServiceId { get; init; } = Guid.NewGuid();
}
