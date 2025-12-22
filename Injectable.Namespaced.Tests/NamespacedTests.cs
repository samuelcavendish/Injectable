using Injectable.Namespaced.Tests.Models;

namespace Injectable.Namespaced.Tests;

public class NamespacedTests
{
    [Fact]
    public void ShouldIncludeNamespacedInjectablesWhenNamespaceIsAdded()
    {
        // The source generator detects the AddNamespace call in InjectableConfiguration.cs
        // at compile time and includes namespaced injectables in the generated code
        var types = Injectables.GetInjectables();
        types.OfServiceType<IPrivateService>().ShouldHaveSingleItem();
    }
}

