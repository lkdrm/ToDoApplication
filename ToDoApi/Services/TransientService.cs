using ToDoApi.Services.Interfaces;

namespace ToDoApi.Services;

/// <summary>
/// Transient implementation of <see cref="ITransientService"/>.
/// A brand-new instance — with a different <see cref="ServiceId"/> — is created
/// for every injection point, even within the same HTTP request.
/// </summary>
public class TransientService : ITransientService
{
    /// <inheritdoc/>
    public Guid ServiceId { get; init; } = Guid.NewGuid();
}
