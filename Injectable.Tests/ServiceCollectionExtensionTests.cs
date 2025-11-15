using Injectable.Tests.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Injectable.Tests
{
    public class ServiceCollectionExtensionTests
    {
        [Fact]
        public void ShouldRegisterSingleton()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddInjectableSingletons(Injectables.GetInjectables());
            var provider = serviceCollection.BuildServiceProvider();

            var singleton = provider.GetRequiredService<Singleton>();
            var transient = provider.GetService<Transient>();
            var scoped = provider.GetService<Scoped>();

            singleton.ShouldNotBeNull();
            singleton.ShouldBeOfType<MySingleton>();
            transient.ShouldBeNull();
            scoped.ShouldBeNull();
        }

        [Fact]
        public void ShouldRegisterTransient()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddInjectableTransients(Injectables.GetInjectables());
            var provider = serviceCollection.BuildServiceProvider();

            var singleton = provider.GetService<Singleton>();
            var transient = provider.GetRequiredService<Transient>();
            var scoped = provider.GetService<Scoped>();

            singleton.ShouldBeNull();
            transient.ShouldNotBeNull();
            transient.ShouldBeOfType<MyTransient>();
            scoped.ShouldBeNull();
        }

        [Fact]
        public void ShouldRegisterScoped()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddInjectableScopes(Injectables.GetInjectables());
            var provider = serviceCollection.BuildServiceProvider();

            var singleton = provider.GetService<Singleton>();
            var transient = provider.GetService<Transient>();
            var scoped = provider.GetRequiredService<Scoped>();

            singleton.ShouldBeNull();
            transient.ShouldBeNull();
            scoped.ShouldNotBeNull();
            scoped.ShouldBeOfType<MyScoped>();
        }

        [Fact]
        public void ShouldRegisterAll()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddInjectables(Injectables.GetInjectables());
            var provider = serviceCollection.BuildServiceProvider();

            var singleton = provider.GetRequiredService<Singleton>();
            var transient = provider.GetRequiredService<Transient>();
            var scoped = provider.GetRequiredService<Scoped>();

            singleton.ShouldNotBeNull();
            singleton.ShouldBeOfType<MySingleton>();
            transient.ShouldNotBeNull();
            transient.ShouldBeOfType<MyTransient>();
            scoped.ShouldNotBeNull();
            scoped.ShouldBeOfType<MyScoped>();
        }
    }
}
