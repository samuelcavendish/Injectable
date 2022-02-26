using Injectable.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System;
using Xunit;

namespace Injectable.Tests;

public class Tests
{
    [Fact]
    public void InjectInterfaceAncestorImplementation()
    {
        var container = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
                services.AddInjectableServices(GetType().Assembly)
        ).Build();
        var service = container.Services.GetRequiredService<InjectConcreteInheritance>();
        Should.Throw<InvalidOperationException>(() => container.Services.GetRequiredService<InjectConcreteInheritanceConcrete>());
    }

    [Fact]
    public void InjectConcreteAncestorImplementation()
    {
        var container = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
                services.AddInjectableServices(GetType().Assembly)
        ).Build();
        var service = container.Services.GetRequiredService<InjectConcreteInheritance>();
        Should.Throw<InvalidOperationException>(() => container.Services.GetRequiredService<InjectConcreteInheritanceConcrete>());
    }
    [Fact]
    public void ConcreteInterfaceAncestorImplementation()
    {
        var container = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
                services.AddInjectableServices(GetType().Assembly)
        ).Build();
        var service = container.Services.GetRequiredService<ConcreteConcreteInheritanceConcrete>();
        Should.Throw<InvalidOperationException>(() => container.Services.GetRequiredService<ConcreteConcreteInheritance>());
    }

    [Fact]
    public void ConcreteConcreteAncestorImplementation()
    {
        var container = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
                services.AddInjectableServices(GetType().Assembly)
        ).Build();
        var service = container.Services.GetRequiredService<ConcreteConcreteInheritanceConcrete>();
        Should.Throw<InvalidOperationException>(() => container.Services.GetRequiredService<ConcreteConcreteInheritance>());
    }

}
