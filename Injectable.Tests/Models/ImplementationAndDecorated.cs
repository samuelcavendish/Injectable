using Injectable.Abstractions;
namespace Injectable.Tests.Models;

[Inject(InjectionType.DecoratedAndImplementation)]
internal class DecoratedAndImplementation { }
internal class DecoratedAndImplementationImplementation : DecoratedAndImplementation { }
