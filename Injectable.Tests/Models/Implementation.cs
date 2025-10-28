using Injectable.Abstractions;

namespace Injectable.Tests.Models;

[Inject(InjectionType.Implementation)]
internal class Implementation { }
internal class ImplementationImplementation : Implementation { }
