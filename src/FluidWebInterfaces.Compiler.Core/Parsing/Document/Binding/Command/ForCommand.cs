namespace FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding.Command;

public class ForCommand : DynamicNode
{
    public string ItemDeclarationType { get; set; } = "";
    public string ItemDeclaration { get; set; } = "";
    public string CollectionDeclaration { get; set; } = "";
    
    public static int ParseFromContent(string content, int i, out ForCommand command)
    {
        command = new ForCommand();
        return i;
    }
}