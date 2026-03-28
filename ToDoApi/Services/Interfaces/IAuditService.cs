namespace ToDoApi.Services.Interfaces;

public interface IAuditService
{
    Task LogActivityAsync(string activity);
}
