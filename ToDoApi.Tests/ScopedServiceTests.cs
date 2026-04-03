using ToDoApi.Services;

namespace ToDoApi.Tests;

public class ScopedServiceTests
{
    [Fact]
    public void ServiceId_IsNotEmpty()
    {
        var service = new ScopedService();

        Assert.NotEqual(Guid.Empty, service.ServiceId);
    }

    [Fact]
    public void ServiceId_IsStableWithinSameInstance()
    {
        var service = new ScopedService();

        Assert.Equal(service.ServiceId, service.ServiceId);
    }

    [Fact]
    public void TwoInstances_HaveDifferentServiceIds()
    {
        var service1 = new ScopedService();
        var service2 = new ScopedService();

        Assert.NotEqual(service1.ServiceId, service2.ServiceId);
    }
}
