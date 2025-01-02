namespace FluidWebInterfaces.Compiler.Core.Compilation;

public class CustomTagDeclaration
{
    public string TagName { get; set; }
    public string Source { get; set; }

    public CustomTagDeclaration(string tagName, string source)
    {
        TagName = tagName;
        Source = source;
    }
}