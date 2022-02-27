namespace Injectable;

public class InjectableType
{
    public Inject Attribute { get; set; } = null!;
    public Type Implementation { get; set; } = null!;
    public Type Service { get; set; } = null!;

    public override string ToString()
    {
        return $"{Service.Name}:{Implementation.Name}:{Attribute.InjectionType}";
    }
}
