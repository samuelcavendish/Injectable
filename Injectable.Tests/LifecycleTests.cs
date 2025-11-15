using Injectable.Tests.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Injectable.Tests
{
    public class LifecycleTests
    {
        [Fact]
        public void Singleton()
        {
            var types = Injectables.GetInjectables();
            types.ShouldContain(x => x.Implementation == typeof(MySingleton) && x.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public void Transient()
        {
            var types = Injectables.GetInjectables();
            types.ShouldContain(x => x.Implementation == typeof(MyTransient) && x.Lifetime == ServiceLifetime.Transient);
        }

        [Fact]
        public void Scoped()
        {
            var types = Injectables.GetInjectables();
            types.ShouldContain(x => x.Implementation == typeof(MyScoped) && x.Lifetime == ServiceLifetime.Scoped);
        }
    }
}
