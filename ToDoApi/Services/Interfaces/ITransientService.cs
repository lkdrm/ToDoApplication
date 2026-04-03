namespace ToDoApi.Services.Interfaces;

/// <summary>
/// Marker interface used to demonstrate <b>Transient</b> service lifetime.
/// A new instance is created for every injection point.
/// </summary>
public interface ITransientService
{
    /// <summary>Unique ID generated once per service instance.</summary>
    Guid ServiceId { get; }
}
