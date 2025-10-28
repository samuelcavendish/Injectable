using Injectable.Tests.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Injectable.Tests
{
    public class InjectableCollectionExtensionTests
    {
        [Fact]
        public void ShouldRegisterSingleton()
        {
            var serviceCollection = new ServiceCollection();
            Injectables.GetInjectables().AddInjectableSingletons(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            var singleton = provider.GetRequiredService<Singleton>();
            var transient = provider.GetService<Transient>();
            var scoped = provider.GetService<Scoped>();

            singleton.ShouldNotBeNull();
            transient.ShouldBeNull();
            scoped.ShouldBeNull();
        }

        [Fact]
        public void ShouldRegisterTransients()
        {
            var serviceCollection = new ServiceCollection();
            Injectables.GetInjectables().AddInjectableTransients(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            var singleton = provider.GetService<Singleton>();
            var transient = provider.GetRequiredService<Transient>();
            var scoped = provider.GetService<Scoped>();

            singleton.ShouldBeNull();
            transient.ShouldNotBeNull();
            scoped.ShouldBeNull();
        }

        [Fact]
        public void ShouldRegisterScoped()
        {
            var serviceCollection = new ServiceCollection();
            Injectables.GetInjectables().AddInjectableScoped(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            var singleton = provider.GetService<Singleton>();
            var transient = provider.GetService<Transient>();
            var scoped = provider.GetRequiredService<Scoped>();

            singleton.ShouldBeNull();
            transient.ShouldBeNull();
            scoped.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldRegisterAll()
        {
            var serviceCollection = new ServiceCollection();
            Injectables.GetInjectables().AddInjectables(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            var singleton = provider.GetRequiredService<Singleton>();
            var transient = provider.GetRequiredService<Transient>();
            var scoped = provider.GetRequiredService<Scoped>();

            singleton.ShouldNotBeNull();
            transient.ShouldNotBeNull();
            scoped.ShouldNotBeNull();
        }
    }
}
