using Injectable.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Injectable;

public static class InjectableTypeExtensions
{
    public static IEnumerable<InjectableType> OfServiceType<T>(this IEnumerable<InjectableType> injectableTypes)
        => injectableTypes.Where(x => x.Service.IsOfType<T>());

    public static IEnumerable<InjectableType> OfImplementationType<T>(this IEnumerable<InjectableType> injectableTypes)
        => injectableTypes.Where(x => x.Implementation.IsOfType<T>());
}
