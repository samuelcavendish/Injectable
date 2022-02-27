namespace Injectable.Tests;

[Inject(InjectionType.FirstGeneric)]
internal class FirstGeneric<T> { }
internal class FirstGenericImplementation : FirstGeneric<FirstGenericImplementation> { }
