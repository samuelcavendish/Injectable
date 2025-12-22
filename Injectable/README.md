# Injectable

`Injectable` is a Roslyn source generator that scans your assemblies for injectable types and emits registration metadata for dependency injection containers.

## What this package does

- **Targets**: `netstandard2.0`
- **Kind**: Analyzer / source generator
- **Discovers**: Types annotated with attributes from `Injectable.Abstractions`
- **Generates**: A list of injectable service registrations that you can plug into your DI container

## Basic usage

1. Add a package reference to **Injectable** and **Injectable.Abstractions** in your project.
2. Annotate services you want to be discovered, for example:

```csharp
using Injectable;

[Injectable]
public class MyService : IMyService { }
```

3. Build your project. The generator will emit injectable metadata (e.g. `Injectables.g.cs`) which you can use to register services in your container.

For more advanced configuration and examples, see the project site or GitHub repository linked from the NuGet page.



