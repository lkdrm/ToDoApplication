using ToDoApi.Services;

namespace ToDoApi.Tests;

public class TransientServiceTests
{
    [Fact]
    public void ServiceId_IsNotEmpty()
    {
        var service = new TransientService();

        Assert.NotEqual(Guid.Empty, service.ServiceId);
    }

    [Fact]
    public void ServiceId_IsStableWithinSameInstance()
    {
        var service = new TransientService();

        Assert.Equal(service.ServiceId, service.ServiceId);
    }

    [Fact]
    public void TwoInstances_HaveDifferentServiceIds()
    {
        var service1 = new TransientService();
        var service2 = new TransientService();

        Assert.NotEqual(service1.ServiceId, service2.ServiceId);
    }
}
