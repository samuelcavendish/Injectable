namespace Injectable.Tests;

public class Tests
{
    private readonly Assembly _assembly = typeof(Tests).Assembly;

    [Fact]
    public void ShouldHaveDecorated()
    {
        var types = InjectableTypes.GetAssemblyInjectables(_assembly);
        types.OfServiceType<IDecorated>().ShouldHaveSingleItem();
        types.OfServiceType<DecoratedImplementation>().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldHaveImplementation()
    {
        var types = InjectableTypes.GetAssemblyInjectables(_assembly);
        _ = types.OfServiceType<Implementation>().Single(x => x.Implementation.IsOfType<Implementation>());
        _ = types.OfServiceType<Implementation>().Single(x => x.Implementation.IsOfType<ImplementationImplementation>());
        types.OfServiceType<ImplementationImplementation>().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldHaveImplementationAndDecorated()
    {
        var types = InjectableTypes.GetAssemblyInjectables(_assembly);
        _ = types.OfServiceType<DecoratedAndImplementation>().Single(x => x.Implementation.IsOfType<DecoratedAndImplementation>());
        _ = types.OfServiceType<DecoratedAndImplementation>().Single(x => x.Implementation.IsOfType<DecoratedAndImplementationImplementation>());
        types.OfServiceType<DecoratedAndImplementationImplementation>().ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldHaveFirstGeneric()
    {
        var types = InjectableTypes.GetAssemblyInjectables(_assembly);
        types.OfServiceType<FirstGenericImplementation>().ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldTraverseInterfacesAndClasses()
    {
        var types = InjectableTypes.GetAssemblyInjectables(_assembly);
        types.OfServiceType<ITraversalInterface>().Count().ShouldBe(2);
        types.OfServiceType<TraversalInterfaceImplementation3>().ShouldHaveSingleItem();
        types.OfServiceType<TraversalInterfaceImplementation>().ShouldHaveSingleItem();
    }
}
