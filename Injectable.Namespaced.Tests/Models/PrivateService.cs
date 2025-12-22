using Injectable.Abstractions;

namespace Injectable.Namespaced.Tests.Models;

[Inject(Namespace = "PrivateServices")]
public interface IPrivateService
{
}

public class PrivateService : IPrivateService
{
}

