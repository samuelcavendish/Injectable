using System.Runtime.CompilerServices;

namespace Injectable.Generator.Tests;

public static class ModuleInitialiser
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}
