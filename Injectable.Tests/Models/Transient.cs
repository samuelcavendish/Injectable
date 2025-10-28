using Injectable.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Injectable.Tests.Models
{
    [Inject(lifecycle: ServiceLifetime.Transient)]
    internal class Transient
    {
    }
}
