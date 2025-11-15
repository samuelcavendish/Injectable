# Injectable
Injectable is a source generator for Microsoft Dependency Injection. Add the `[Inject]` attribute to an interface/class and the incremental generator will emit a list of injectable mappings at compile time. The injectable type doesn't have to live in the same assembly as the implementation and Injectable will keep traversing base types/interfaces so even if the `[Inject]` attribute is on an "Ancestor" the type will still be detected to be injected.

Injectable produces a list of injectable types which can then be used like so:

```
foreach (var injectable in Injectables.GetInjectables())
{
    services.AddSingleton(injectable.Service, injectable.Implementation);
}
```

By default Injectable will surface every implementation discovered during compilation. If you want to just register certain types you can filter the `Injectables.GetInjectables()` results, for example, `Injectables.GetInjectables().OfServiceType<IMyInterface>()`. You can use this functionality to choose how services are registered (e.g. register some services as Singleton & others as Transient).

### Namespaced injectables

If you want to keep some injectables private to a solution, you can supply an optional namespace when applying the attribute:

```
[Inject(Namespace = "PrivateServices")]
public interface IInternalService { }

public class InternalService : IInternalService { }
```

Injectable will skip generating these entries unless the project contains a call to `Injectables.AddNamespace("PrivateServices");`. Simply invoke the helper once (for example in your composition root) to opt-in:

```
Injectables.AddNamespace("PrivateServices");
```

Namespaces are opt-in per project, so a shared library can hide private registrations while still exposing them in other solutions that choose to opt-in.

## What can I register

Injectable offers 4 ways to set the service type that's registered

### Decorated (default)

Registers Decorated type as the Service making 

```
[Inject]
public interface IInterface {}

public class Implementation : IInterface {} 
```

Equivalent to

```
services.AddSingleton<IInterface, Implementation>();
```

### Implementation

Registers Implementation type as the Service making 

```
[Inject(As = InjectType.Implementation)]
public interface IInterface {}

public class Implementation : IInterface {} 
```

Equivalent to

```
services.AddSingleton<Implementation>();
```

### Decorated Implementation

Registers both Decorated and Implementation type as the Service making 

```
[Inject(As = InjectType.DecoratedAndImplementation)]
public interface IInterface {}

public class Implementation : IInterface {} 
```

Equivalent to

```
services.AddSingleton<IInterface, Implementation>();
services.AddSingleton<Implementation>();
```

### First Generic

The first generic type allows you to mark a generic interface/class to inject as a different type (From the first generic argument). This allows Auto-Injection even for Services you can't add the Inject attribute to directly and also to register items without a common interface. This is for use cases such as having multiple "repositories" that have no common interface, e.g. ItemReadRepository, ItemWriteRepository.

```
[Inject(As = InjectType.FirstGeneric)]
public interface IRepository<T> { }
public class ItemReadRepository : IRepository<ItemReadRepository> { }
public class ItemWriteRepository : IRepository<ItemWriteRepository> { }
```

Is equivalent to

```
services.AddSingleton<ItemReadRepository, ItemReadRepository>();
services.AddSingleton<ItemWriteRepository, ItemWriteRepository>();
```

Using this setup, the repositories can be retrieved from DI like normal

```
services.GetRequiredService<ItemReadRepository>();
```

### Multiple implementations

Injectable, by default, puts the power in your control by providing you the list to register instead of doing the registration for you. This means there can be duplicate service registrations within the list returned by Injectable. Microsoft DI by default has a "Last wins" implementation so calling 

```
services.AddSingleton<IInterface, Implementation1>();
services.AddSingleton<IInterface, Implementation2>();
```

Would result in `Implementation2` being retrieved when requesting IInterface. Injectable doesn't do any work to try and resolve this, that would be down to the user as Injectable has no way of determining the order. The multiple registration does however mean you can take advantage of desired multiple registrations e.g.

```
[Inject]
public interface IMessageHandler { }
public class MessageHandler1 : IMessageHandler { }
public class MessageHandler2 : IMessageHandler { }
```

This would lead to two registrations that can be retrieved & used by Microsoft DI e.g.

```
var messageHandlers = service.GetRequiredService<IEnumerable<IMessageHandler>>();
foreach (var messageHandler in messageHandlers) {
    messageHandler.Handle(message);
}
```

### Putting the power in your hands

When authoring a library like this, it's imposible to cater to everyones expectations. Lets take an example:

```
[Inject]
public MyClass { }
public MyDerivedClass { }
```

For some users, they'd expect MyClass to be injected (which it is by default). For others, they may only expect MyDerivedClass to be injected. I could of tried handling this in the library, added a flag on the inject attribute for IncludeDecorated, but the user can make these decisions themselves.

In this case the user could filter the results that come back from Injectable before registering the types, like so:

```
var injectables = Injectables.GetInjectables()
    .Where(x => x.Implementation.GetCustomAttribute<InjectAttribute>() is null);

// Requires:
// using System.Reflection;
```

As easy as that we've filtered out any implementations that have the Inject attribute on directly

### Registering Injectables

To make it faster to add Injectables to your container, Injectable provides you with 3 extension methods on IServiceCollection

```
.AddInjectableSingletons
.AddInjectableTransients
.AddInjectableScopes
```

These 3 extensions allow you to add all the results from Injectables at once instead of having to loop through the results