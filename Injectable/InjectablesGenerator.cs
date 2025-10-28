using Injectable.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Injectable;

[Generator]
public class InjectablesGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new InjectableSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not InjectableSyntaxReceiver receiver)
            return;

        var compilation = context.Compilation;

        // Get all classes marked with [Inject] attribute
        var injectableTypes = new List<(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, int type, int lifecycle)>();

        foreach (var candidate in receiver.Candidates.Concat<SyntaxNode>(receiver.InterfaceCandidates))
        {
            var semanticModel = compilation.GetSemanticModel(candidate.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(candidate);

            if (symbol == null || !(symbol is INamedTypeSymbol namedSymbol))
                continue;

            // Check if class or interface has [Inject] attribute
            var injectAttribute = GetInjectAttribute(namedSymbol);

            if (injectAttribute != null)
            {
                var injectionType = GetInjectionType(injectAttribute);
                var lifecycle = GetLifecycle(injectAttribute);
                injectableTypes.Add((namedSymbol, namedSymbol, injectionType, lifecycle));
            }
        }

        // Find implementations
        var implementations = new List<(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, int type, int lifecycle)>();

        foreach (var typeSymbol in GetAllTypes(compilation.GlobalNamespace))
        {
            if (!(typeSymbol is INamedTypeSymbol namedType))
                continue;

            if (namedType.IsAbstract || namedType.IsStatic)
                continue;

            if (namedType.TypeKind != TypeKind.Class && namedType.TypeKind != TypeKind.Struct)
                continue;

            // Check base types and interfaces for [Inject] attribute
            var serviceTypes = GetAllAncestorTypes(namedType);

            foreach (var serviceType in serviceTypes)
            {
                var attribute = GetInjectAttribute(serviceType);
                if (attribute != null)
                {
                    var injectionType = GetInjectionType(attribute);
                    var lifecycle = GetLifecycle(attribute);
                    implementations.Add((serviceType, namedType, injectionType, lifecycle));

                    if (injectionType == 3) // DecoratedAndImplementation
                    {
                        implementations.Add((namedType, namedType, injectionType, lifecycle));
                    }
                }
            }

            // Handle FirstGeneric
            if (serviceTypes.Any(st => GetInjectionType(GetInjectAttribute(st)) == 4 && st.IsGenericType))
            {
                var genericServiceType = serviceTypes.FirstOrDefault(st => GetInjectionType(GetInjectAttribute(st)) == 4 && st.IsGenericType);
                var genericAttribute = GetInjectAttribute(genericServiceType);
                if (genericServiceType != null && genericServiceType.IsGenericType && genericServiceType.TypeArguments.Length > 0)
                {
                    var firstGenericArg = genericServiceType.TypeArguments[0];
                    if (firstGenericArg is INamedTypeSymbol firstArg && genericAttribute != null)
                    {
                        var lifecycle = GetLifecycle(genericAttribute);
                        implementations.Add((firstArg, namedType, 4, lifecycle)); // FirstGeneric
                    }
                }
            }
        }

        // Remove duplicates by keeping only the most specific (last) ones
        var distinctImplementations = implementations
            .GroupBy(impl => new { ServiceKey = impl.serviceType.ToDisplayString(), ImplementationKey = impl.implementationType.ToDisplayString() })
            .Select(g => g.Last())
            .ToList();

        if (distinctImplementations.Count == 0)
            return;

        // Generate source code
        var source = GenerateSource(compilation, distinctImplementations);
        context.AddSource("Injectables.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static IEnumerable<INamedTypeSymbol> GetAllAncestorTypes(INamedTypeSymbol type)
    {
        var types = new List<INamedTypeSymbol>
        {
            // Add current type
            type
        };

        // Add base class hierarchy
        var baseType = type.BaseType;
        while (baseType != null && baseType.Name != "Object")
        {
            types.Add(baseType);
            baseType = baseType.BaseType;
        }

        // Add all interfaces
        foreach (var interfaceType in type.AllInterfaces)
        {
            if (interfaceType is INamedTypeSymbol namedInterface)
            {
                types.Add(namedInterface);

                // Add parent interfaces
                foreach (var parentInterface in GetAllInterfaces(namedInterface))
                {
                    types.Add(parentInterface);
                }
            }
        }

        return types.Distinct(SymbolEqualityComparer.Default).OfType<INamedTypeSymbol>();
    }

    private static IEnumerable<INamedTypeSymbol> GetAllInterfaces(INamedTypeSymbol interfaceType)
    {
        var result = new List<INamedTypeSymbol>();

        foreach (var baseInterface in interfaceType.Interfaces)
        {
            if (baseInterface is INamedTypeSymbol namedBase)
            {
                result.Add(namedBase);
                result.AddRange(GetAllInterfaces(namedBase));
            }
        }

        return result;
    }

    private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            yield return type;

            foreach (var nestedType in type.GetTypeMembers())
            {
                yield return nestedType;
            }
        }

        foreach (var childNamespace in ns.GetNamespaceMembers())
        {
            foreach (var type in GetAllTypes(childNamespace))
            {
                yield return type;
            }
        }
    }

    private static AttributeData? GetInjectAttribute(INamedTypeSymbol symbol)
    {
        return symbol.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.Name == nameof(InjectAttribute) || attr.AttributeClass?.Name == "Inject");
    }

    private static int GetInjectionType(AttributeData? attribute)
    {
        if (attribute == null)
            return 1; // Decorated

        if (attribute.NamedArguments.IsEmpty && attribute.ConstructorArguments.IsEmpty)
            return 1; // Decorated

        if (attribute.ConstructorArguments.Length > 0)
        {
            var arg = attribute.ConstructorArguments[0];
            if (arg.Kind == TypedConstantKind.Enum)
            {
                return (int)arg.Value!;
            }
        }

        // Check named arguments
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == nameof(InjectionType) && namedArg.Value.Kind == TypedConstantKind.Enum)
            {
                return (int)namedArg.Value.Value!;
            }
        }

        return 1; // Decorated
    }

    private static int GetLifecycle(AttributeData? attribute)
    {
        if (attribute == null)
            return 1; // Singleton

        // Check named arguments first
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "Lifecycle" && namedArg.Value.Kind == TypedConstantKind.Enum)
            {
                return (int)namedArg.Value.Value!;
            }
        }

        // Check constructor arguments (second parameter)
        if (attribute.ConstructorArguments.Length > 1)
        {
            var arg = attribute.ConstructorArguments[1];
            if (arg.Kind == TypedConstantKind.Enum)
            {
                return (int)arg.Value!;
            }
        }

        return 1; // Singleton
    }

    private string GenerateSource(Compilation compilation, List<(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, int type, int lifecycle)> implementations)
    {
        var template = EmbeddedResourceReader.ReadAsString(this.GetType().Assembly, "Injectable.Resources.Injectables.txt");
        var implementationString = new StringBuilder();

        foreach (var impl in implementations)
        {
            var serviceType = ToFullyQualifiedTypeName(impl.serviceType);
            var implementationType = ToFullyQualifiedTypeName(impl.implementationType);
            var injectionType = impl.type;
            var lifecycle = impl.lifecycle;
            var enumName = injectionType switch
            {
                1 => "Decorated",
                2 => "Implementation",
                3 => "DecoratedAndImplementation",
                4 => "FirstGeneric",
                _ => "Decorated"
            };
            var lifecycleName = lifecycle switch
            {
                0 => "Singleton",
                1 => "Scoped",
                2 => "Transient",
                _ => "Singleton"
            };

            implementationString.AppendLine($"            yield return new InjectableType");
            implementationString.AppendLine($"            {{");
            implementationString.AppendLine($"                Service = typeof({serviceType}),");
            implementationString.AppendLine($"                Implementation = typeof({implementationType}),");
            implementationString.AppendLine($"                Attribute = new InjectAttribute(InjectionType.{enumName}, ServiceLifetime.{lifecycleName})");
            implementationString.AppendLine($"            }};");
        }

        return template.Replace("{{ Implementations }}", implementationString.ToString());
    }

    private string ToFullyQualifiedTypeName(INamedTypeSymbol symbol)
    {
        if (symbol.IsGenericType)
        {
            var nameWithoutArity = symbol.Name.Split('`')[0];
            return $"{symbol.ContainingNamespace.ToDisplayString()}.{nameWithoutArity}<{string.Join(", ", symbol.TypeArguments.OfType<INamedTypeSymbol>().Select(ToFullyQualifiedTypeName))}>";
        }

        return $"{symbol.ContainingNamespace.ToDisplayString()}.{symbol.Name}";
    }
}
