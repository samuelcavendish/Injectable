namespace Injectable.Extensions;

public static class InjectableTypeExtensions
{
    public static IEnumerable<InjectableType> OfServiceType<T>(this IEnumerable<InjectableType> injectableTypes)
        => injectableTypes.Where(x => x.Service.IsOfType<T>());
}
