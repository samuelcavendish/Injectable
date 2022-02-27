namespace Injectable;

public class InjectableType
{
    public Inject Attribute { get; set; } = null!;
    public Type Implementation { get; set; } = null!;
    public Type Service { get; set; } = null!;
}
