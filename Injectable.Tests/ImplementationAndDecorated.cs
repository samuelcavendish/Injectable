namespace Injectable.Tests;

[Inject(InjectionType.DecoratedAndImplementation)]
internal class DecoratedAndImplementation { }
internal class DecoratedAndImplementationImplementation : DecoratedAndImplementation { }
