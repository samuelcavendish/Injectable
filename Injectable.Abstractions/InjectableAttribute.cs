using Microsoft.Extensions.DependencyInjection;
using System;

namespace Injectable.Abstractions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public class InjectAttribute : Attribute
{
    public InjectAttribute(InjectionType injectionType = InjectionType.Decorated, ServiceLifetime lifecycle = ServiceLifetime.Singleton)
    {
        InjectionType = injectionType;
        Lifecycle = lifecycle;
    }

    public InjectionType InjectionType { get; }
    public ServiceLifetime Lifecycle { get; }
}
