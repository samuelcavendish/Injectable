using Injectable.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Injectable.Generator.Tests;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        // Parse the provided string into a C# syntax tree
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        // https://stackoverflow.com/questions/23907305/roslyn-has-no-reference-to-system-runtime
        var dotNetAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)
            ?? throw new NullReferenceException("Object.Assembly.Location");

        // Create references for assemblies we require
        // We could add multiple references if required
        IEnumerable<PortableExecutableReference> references = new[]
        {
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "netstandard.dll")),  // .NET assemblies are finicky and need to be loaded in a special way.
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Private.CoreLib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(InjectAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.Extensions.DependencyInjection.ServiceLifetime).Assembly.Location)
            };

        // Create a Roslyn compilation for the syntax tree.
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests.Generator",
            syntaxTrees: new[] { syntaxTree });
        compilation = compilation.WithReferences(references);

        // Create an instance of our EnumGenerator incremental source generator
        var generator = new InjectablesGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        // Use verify to snapshot test the source generator output!
        return Verifier
            .Verify(driver)
            .UseDirectory("Snapshots");
    }
}