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
            types.ShouldContain(x => x.Implementation == typeof(Singleton) && x.Lifecycle == ServiceLifetime.Singleton);
        }

        [Fact]
        public void Transient()
        {
            var types = Injectables.GetInjectables();
            types.ShouldContain(x => x.Implementation == typeof(Transient) && x.Lifecycle == ServiceLifetime.Transient);
        }

        [Fact]
        public void Scoped()
        {
            var types = Injectables.GetInjectables();
            types.ShouldContain(x => x.Implementation == typeof(Scoped) && x.Lifecycle == ServiceLifetime.Scoped);
        }
    }
}
