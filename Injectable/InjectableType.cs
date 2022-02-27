namespace Injectable;

public class InjectableType
{
    public Inject Attribute { get; init; } = null!;
    public Type Implementation { get; init; } = null!;
    public Type Service { get; init; } = null!;

    public override string ToString()
    {
        return $"{Service.Name}:{Implementation.Name}:{Attribute.InjectionType}";
    }
}
