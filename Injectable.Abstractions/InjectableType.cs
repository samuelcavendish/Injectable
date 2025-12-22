using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injectable.Abstractions;

public class InjectableType
{
    public InjectAttribute Attribute { get; set; } = null!;
    public Type Implementation { get; set; } = null!;
    public Type Service { get; set; } = null!;

    public ServiceLifetime Lifetime => Attribute.Lifetime;
}
