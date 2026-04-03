namespace ToDoApi.Services.Interfaces;

/// <summary>
/// Marker interface used to demonstrate <b>Scoped</b> service lifetime.
/// All injection points within the same HTTP request share one instance.
/// </summary>
public interface IScopedService
{
    /// <summary>
    /// Unique identifier generated once when the instance is created.
    /// Within a single request every injection of <see cref="IScopedService"/> returns the same value.
    /// </summary>
    Guid ServiceId { get; }
}
