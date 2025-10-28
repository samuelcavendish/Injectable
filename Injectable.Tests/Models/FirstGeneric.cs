using Injectable.Abstractions;

namespace Injectable.Tests.Models;

[Inject(InjectionType.FirstGeneric)]
internal class FirstGeneric<T> { }
internal class FirstGenericImplementation : FirstGeneric<FirstGenericImplementation> { }
