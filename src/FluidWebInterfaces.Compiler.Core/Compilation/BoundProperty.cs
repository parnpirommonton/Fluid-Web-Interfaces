namespace FluidWebInterfaces.Compiler.Core;

public class BoundProperty
{
    public Guid Id { get; set; }
    public string Value { get; set; }
    public string Attribute { get; set; }

    public BoundProperty(Guid id, string value, string attribute)
    {
        Id = id;
        Value = value;
        Attribute = attribute;
    }
}