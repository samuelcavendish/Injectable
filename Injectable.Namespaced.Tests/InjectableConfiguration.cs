namespace Injectable.Tests.Namespaced;

// This file contains a call to AddNamespace that the source generator will detect
// during compilation. This allows namespaced injectables to be included in the generated code.
static partial class InjectableConfiguration
{
    static InjectableConfiguration()
    {
        // This call is detected by the source generator at compile time
        Injectables.AddNamespace("PrivateServices");
    }
}

