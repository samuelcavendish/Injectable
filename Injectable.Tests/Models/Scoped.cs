using Injectable.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Injectable.Tests.Models
{
    [Inject(lifetime: ServiceLifetime.Scoped)]
    internal class Scoped { }
    internal class MyScoped : Scoped { }
}
