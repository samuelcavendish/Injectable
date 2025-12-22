# Injectable.Abstractions

`Injectable.Abstractions` contains the public attributes and types used by the **Injectable** source generator.

## What this package provides

- **Targets**: `netstandard2.0`
- **Contains**:
  - Marker attributes such as `InjectableAttribute`
  - Supporting types such as `InjectableType` and related enums
- **Used by**: Your application code and the `Injectable` generator

## Basic usage

1. Add a package reference to **Injectable.Abstractions** in any project where you want to mark injectable services.
2. Annotate your services, for example:

```csharp
using Injectable;

[Injectable]
public class MyService : IMyService { }
```

3. Add a reference to the **Injectable** generator package in your consuming project to have those attributes picked up at compile time.

See the `Injectable` package description and the linked project site/GitHub repo on NuGet for more details and advanced scenarios.



