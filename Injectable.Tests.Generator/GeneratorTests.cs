namespace Injectable.Generator.Tests;

public class GeneratorTests
{
    [Fact]
    public async Task Should_InjectAsDecorated_When_NoInjectionType()
    {
        var source = """"
            using Injectable;
            using Injectable.Abstractions;

            [Inject]
            public interface IInjectableInterface {}
            public class MyInjectableClass : IInjectableInterface { }
        """";
        await TestHelper.Verify(source);
    }

    [Fact]
    public async Task Should_InjectAsImplementation_When_TypeIsImplementation()
    {
        var source = """"
            using Injectable;
            using Injectable.Abstractions;

            [Inject(InjectionType.Implementation)]
            internal class Implementation { }
            internal class ImplementationImplementation : Implementation { }        
        """";
        await TestHelper.Verify(source);
    }

    [Fact]
    public async Task Should_InjectAsFirstGeneric_When_TypeIsFirstGeneric()
    {
        var source = """"
            using Injectable.Abstractions;

            [Inject(InjectionType.FirstGeneric)]
            internal interface FirstGeneric<T> { }
            internal class FirstGenericImplementation : FirstGeneric<FirstGenericImplementation> { }               
        """";
        await TestHelper.Verify(source);
    }


    [Fact]
    public async Task Should_InjectAsDecoratedAndImplementation_When_TypeIsDecoratedAndImplementation()
    {
        var source = """"
            using Injectable.Abstractions;

            [Inject(InjectionType.DecoratedAndImplementation)]
            internal class DecoratedAndImplementation { }
            internal class DecoratedAndImplementationImplementation : DecoratedAndImplementation { }                    
        """";
        await TestHelper.Verify(source);
    }
}

