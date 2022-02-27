namespace Injectable;

public static class InjectableTypes
{
    public static IEnumerable<InjectableType> GetAssemblyInjectables(Assembly assembly)
    {
        IEnumerable<InjectableType> GetAssemblyInjectablesInternal()
        {
            var injectableImplementations = assembly.DefinedTypes
                .Where(x => x is { IsAbstract: false, IsClass: true, IsInterface: false });

            IEnumerable<Type> getBaseType(Type t) => t.BaseType is { BaseType.IsInterface: false } ? new[] { t.BaseType } : Enumerable.Empty<Type>();
            IEnumerable<Type> getInterface(Type t) => t.GetInterfaces();

            foreach (var possibleInjectable in injectableImplementations)
            {
                var types = TypeWithRecursiveParents(possibleInjectable, getBaseType);
                types = types.Concat(types.SelectMany(x => TypeWithRecursiveParents(x, getInterface)));

                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<Inject>();
                    if (attribute is { InjectionType: InjectionType.Decorated or Injectable.InjectionType.Implementation or InjectionType.DecoratedAndImplementation })
                        yield return new InjectableType { Attribute = attribute, Implementation = possibleInjectable, Service = type };

                    if (attribute is { InjectionType: InjectionType.DecoratedAndImplementation })
                        yield return new InjectableType { Attribute = attribute, Implementation = possibleInjectable, Service = possibleInjectable };

                    if (attribute is { InjectionType: InjectionType.FirstGeneric } && type is { IsGenericType: true })
                    {
                        var firstGenericType = type.GenericTypeArguments.FirstOrDefault();
                        if (firstGenericType is not null)
                            yield return new InjectableType { Attribute = attribute, Implementation = possibleInjectable, Service = firstGenericType };
                    }

                }
            }
        }

        // Return in reverse so the "lowest" class ancestor wins
        return GetAssemblyInjectablesInternal().Reverse().DistinctBy(x => new { x.Implementation, x.Service });
    }

    private static IEnumerable<Type> TypeWithRecursiveParents(Type current, Func<Type, IEnumerable<Type>> getParents)
    {
        yield return current;
        foreach (var parent in getParents(current).SelectMany(x => TypeWithRecursiveParents(x, getParents)))
        {
            yield return parent;
        }
    }
}
