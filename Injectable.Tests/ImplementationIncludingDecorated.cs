namespace Injectable.Tests
{

    [Inject(InjectionType.Implementation, true)]
    internal class ImplementationIncludingDecorated { }
    internal class ImplementationIncludingDecoratedImplementation : ImplementationIncludingDecorated { }
}
