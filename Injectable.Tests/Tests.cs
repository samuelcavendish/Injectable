using Injectable.Extensions;
using Shouldly;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Injectable.Tests;

public class Tests
{
    private readonly Assembly _assembly = typeof(Tests).Assembly;

    [Fact]
    public void ShouldHaveDecorated()
    {
        var types = InjectableTypeRepository.GetAssemblyInjectables(_assembly);
        types.OfServiceType<IDecorated>().ShouldHaveSingleItem();
        types.OfServiceType<DecoratedImplementation>().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldHaveImplementation()
    {
        var types = InjectableTypeRepository.GetAssemblyInjectables(_assembly);
        types.OfServiceType<Implementation>().ShouldHaveSingleItem();
        types.OfServiceType<ImplementationImplementation>().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldHaveImplementationAndDecorated()
    {
        var types = InjectableTypeRepository.GetAssemblyInjectables(_assembly);
        types.OfServiceType<DecoratedAndImplementation>().ShouldHaveSingleItem();
        types.OfServiceType<DecoratedAndImplementationImplementation>().ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldHaveFirstGeneric()
    {
        var types = InjectableTypeRepository.GetAssemblyInjectables(_assembly);
        types.OfServiceType<FirstGenericImplementation>().ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldTraverseInterfacesAndClasses()
    {
        var types = InjectableTypeRepository.GetAssemblyInjectables(_assembly);
        types.OfServiceType<ITraversalInterface>().Count().ShouldBe(2);
        types.OfServiceType<TraversalInterfaceImplementation3>().ShouldHaveSingleItem();
        types.OfServiceType<TraversalInterfaceImplementation>().ShouldHaveSingleItem();
    }
}
