namespace FluidWebInterfaces.Compiler.Core.Parsing.Document;

public abstract class DocumentNode : INode
{
    public List<INode> Children { get; set; } = [];
}