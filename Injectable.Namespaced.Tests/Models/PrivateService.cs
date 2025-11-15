using Injectable.Abstractions;

namespace Injectable.Tests.Namespaced.Models;

[Inject(Namespace = "PrivateServices")]
public interface IPrivateService
{
}

public class PrivateService : IPrivateService
{
}

