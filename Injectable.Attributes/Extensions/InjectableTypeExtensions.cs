using Injectable.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Injectable;

public static class InjectableTypeExtensions
{
    public static IEnumerable<InjectableType> OfServiceType<T>(this IEnumerable<InjectableType> injectableTypes)
        => injectableTypes.Where(x => x.Service == typeof(T));

    public static IEnumerable<InjectableType> OfImplementationType<T>(this IEnumerable<InjectableType> injectableTypes)
        => injectableTypes.Where(x => x.Implementation == typeof(T));

    public static IEnumerable<InjectableType> WithLifecycle(this IEnumerable<InjectableType> injectableTypes, ServiceLifetime lifecycle)
        => injectableTypes.Where(x => x.Lifecycle == lifecycle);

    public static IEnumerable<InjectableType> WithSingletonLifecycle(this IEnumerable<InjectableType> injectableTypes)
        => injectableTypes.WithLifecycle(ServiceLifetime.Singleton);

    public static IEnumerable<InjectableType> WithScopedLifecycle(this IEnumerable<InjectableType> injectableTypes)
        => injectableTypes.WithLifecycle(ServiceLifetime.Scoped);

    public static IEnumerable<InjectableType> WithTransientLifecycle(this IEnumerable<InjectableType> injectableTypes)
        => injectableTypes.WithLifecycle(ServiceLifetime.Transient);
}
