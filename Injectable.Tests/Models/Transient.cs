using Injectable.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Injectable.Tests.Models
{
    [Inject(lifetime: ServiceLifetime.Transient)]
    internal class Transient
    {
    }

    internal class MyTransient : Transient { }
}
