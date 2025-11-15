using Injectable.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Injectable;

[Generator]
public class InjectablesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Collect all AddNamespace calls from the syntax tree.
        // Order guarantee: The syntax provider runs first, collecting all namespace requests,
        // then Combine() ensures these are available before processing the compilation.
        // This guarantees that namespaced injectables are filtered correctly based on AddNamespace calls.
        var namespaceRequests = context.SyntaxProvider
            .CreateSyntaxProvider(static (node, _) => node is InvocationExpressionSyntax invocation && IsAddNamespaceInvocation(invocation),
                static (context, _) => GetNamespacesFromInvocation(context))
            .SelectMany(static (namespaces, _) => namespaces)
            .Collect();

        // Combine compilation with namespace requests. The Combine() ensures namespaceRequests
        // is fully collected before the compilation is processed, maintaining order guarantee.
        var implementationsProvider = context.CompilationProvider
            .Combine(namespaceRequests)
            .Select(static (data, cancellationToken) =>
            {
                var (compilation, namespaceLiterals) = data;
                var allowedNamespaces = namespaceLiterals.IsDefaultOrEmpty
                    ? ImmutableHashSet<string>.Empty
                    : namespaceLiterals.ToImmutableHashSet(StringComparer.Ordinal);

                return ComputeImplementations(compilation, allowedNamespaces, cancellationToken);
            });

        context.RegisterSourceOutput(implementationsProvider, (productionContext, implementations) =>
        {
            if (implementations.IsDefaultOrEmpty)
            {
                return;
            }

            var source = GenerateSource(implementations);
            productionContext.AddSource("Injectables.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    private static ImmutableArray<(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, int type, int lifetime)> ComputeImplementations(
        Compilation compilation,
        ImmutableHashSet<string> allowedNamespaces,
        CancellationToken cancellationToken)
    {
        var implementations = new List<(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, int type, int lifetime)>();

        foreach (var typeSymbol in GetAllTypes(compilation.GlobalNamespace))
        {
            cancellationToken.ThrowIfCancellationRequested();

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
                    var namespaceFilter = GetNamespace(attribute);
                    if (!ShouldIncludeNamespace(namespaceFilter, allowedNamespaces))
                    {
                        continue;
                    }

                    var injectionType = GetInjectionType(attribute);
                    var lifetime = GetLifecycle(attribute);
                    if (injectionType is 1 or 2 or 3)
                        implementations.Add((serviceType, namedType, injectionType, lifetime));

                    if (injectionType == 3) // DecoratedAndImplementation
                    {
                        implementations.Add((namedType, namedType, injectionType, lifetime));
                    }
                }
            }

            // Handle FirstGeneric
            if (serviceTypes.Any(st =>
            {
                var attribute = GetInjectAttribute(st);
                if (attribute == null || GetInjectionType(attribute) != 4 || !st.IsGenericType)
                {
                    return false;
                }

                return ShouldIncludeNamespace(GetNamespace(attribute), allowedNamespaces);
            }))
            {
                var genericServiceType = serviceTypes.FirstOrDefault(st =>
                {
                    var attribute = GetInjectAttribute(st);
                    if (attribute == null || GetInjectionType(attribute) != 4 || !st.IsGenericType)
                    {
                        return false;
                    }

                    return ShouldIncludeNamespace(GetNamespace(attribute), allowedNamespaces);
                });

                var genericAttribute = genericServiceType != null ? GetInjectAttribute(genericServiceType) : null;

                if (genericServiceType != null && genericServiceType.IsGenericType && genericServiceType.TypeArguments.Length > 0)
                {
                    var firstGenericArg = genericServiceType.TypeArguments[0];
                    if (firstGenericArg is INamedTypeSymbol firstArg && genericAttribute != null)
                    {
                        if (!ShouldIncludeNamespace(GetNamespace(genericAttribute), allowedNamespaces))
                        {
                            continue;
                        }

                        var lifetime = GetLifecycle(genericAttribute);
                        implementations.Add((firstArg, namedType, 4, lifetime)); // FirstGeneric
                    }
                }
            }
        }

        // Remove duplicates by keeping only the most specific (last) ones
        var distinctImplementations = implementations
            .GroupBy(impl => new { ServiceKey = impl.serviceType.ToDisplayString(), ImplementationKey = impl.implementationType.ToDisplayString() })
            .Select(g => g.Last())
            .ToImmutableArray();

        if (distinctImplementations.IsDefaultOrEmpty)
            return ImmutableArray<(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, int type, int lifetime)>.Empty;

        return distinctImplementations;
    }

    private static IEnumerable<INamedTypeSymbol> GetAllAncestorTypes(INamedTypeSymbol type)
    {
        // Don't include the decorated type, it's just the marker
        var types = new List<INamedTypeSymbol>();

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

    private static bool IsAddNamespaceInvocation(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return false;
        }

        if (!string.Equals(memberAccess.Name.Identifier.ValueText, "AddNamespace", StringComparison.Ordinal))
        {
            return false;
        }

        return memberAccess.Expression switch
        {
            IdentifierNameSyntax identifier when identifier.Identifier.ValueText == "Injectables" => true,
            MemberAccessExpressionSyntax innerMember => innerMember.ToString().EndsWith(".Injectables", StringComparison.Ordinal),
            _ => false
        };
    }

    private static ImmutableArray<string> GetNamespacesFromInvocation(GeneratorSyntaxContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var namespaces = ImmutableArray.CreateBuilder<string>();

        foreach (var argument in invocation.ArgumentList.Arguments)
        {
            var constantValue = context.SemanticModel.GetConstantValue(argument.Expression);
            if (constantValue.HasValue && constantValue.Value is string value && !string.IsNullOrWhiteSpace(value))
            {
                namespaces.Add(value);
            }
        }

        return namespaces.ToImmutable();
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
            if (namedArg.Key == "lifetime" && namedArg.Value.Kind == TypedConstantKind.Enum)
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

    private static string? GetNamespace(AttributeData? attribute)
    {
        if (attribute == null)
        {
            return null;
        }

        // Check named arguments (e.g., Namespace = "Private")
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == nameof(InjectAttribute.Namespace) && namedArg.Value.Value is string namedValue)
            {
                return namedValue;
            }
        }

        // Check constructor arguments (third parameter)
        if (attribute.ConstructorArguments.Length > 2)
        {
            var arg = attribute.ConstructorArguments[2];
            if (arg.Value is string ctorValue)
            {
                return ctorValue;
            }
        }

        return null;
    }

    private static bool ShouldIncludeNamespace(string? namespaceValue, ImmutableHashSet<string> allowedNamespaces)
    {
        if (string.IsNullOrWhiteSpace(namespaceValue))
        {
            return true;
        }

        return allowedNamespaces.Contains(namespaceValue!);
    }

    private static string GenerateSource(ImmutableArray<(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType, int type, int lifetime)> implementations)
    {
        var template = EmbeddedResourceReader.ReadAsString(typeof(InjectablesGenerator).Assembly, "Injectable.Resources.Injectables.txt");
        var implementationString = new StringBuilder();

        foreach (var impl in implementations)
        {
            var serviceType = ToFullyQualifiedTypeName(impl.serviceType);
            var implementationType = ToFullyQualifiedTypeName(impl.implementationType);
            var injectionType = impl.type;
            var lifetime = impl.lifetime;
            var enumName = injectionType switch
            {
                1 => "Decorated",
                2 => "Implementation",
                3 => "DecoratedAndImplementation",
                4 => "FirstGeneric",
                _ => "Decorated"
            };
            var lifecycleName = lifetime switch
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

    private static string ToFullyQualifiedTypeName(INamedTypeSymbol symbol)
    {
        if (symbol.IsGenericType)
        {
            var nameWithoutArity = symbol.Name.Split('`')[0];
            return $"{GetNamespacePrefix(symbol)}{nameWithoutArity}<{string.Join(", ", symbol.TypeArguments.OfType<INamedTypeSymbol>().Select(ToFullyQualifiedTypeName))}>";
        }

        return $"{GetNamespacePrefix(symbol)}{symbol.Name}";
    }

    private static string GetNamespacePrefix(INamedTypeSymbol symbol)
    {
        var namespaceName = symbol.ContainingNamespace.ToDisplayString();
        return string.IsNullOrEmpty(namespaceName) ? string.Empty : namespaceName + ".";
    }
}
