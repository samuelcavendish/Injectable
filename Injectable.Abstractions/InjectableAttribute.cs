using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injectable.Abstractions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public class InjectAttribute : Attribute
{
    public InjectAttribute(InjectionType injectionType = InjectionType.Decorated, ServiceLifetime lifetime = ServiceLifetime.Singleton, string? @namespace = null)

    {
        InjectionType = injectionType;
        Lifetime = lifetime;
        Namespace = @namespace;
    }

    public InjectionType InjectionType { get; }
    public ServiceLifetime Lifetime { get; }
    public string? Namespace { get; set; }
}
