namespace FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding;

public class BindableContentNode : INode
{
    public NodeContentType ContentType;
    public string StringValue = "";
    public string VariableReference = "";
}