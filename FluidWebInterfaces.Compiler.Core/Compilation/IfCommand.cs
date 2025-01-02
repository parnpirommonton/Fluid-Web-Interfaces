namespace FluidWebInterfaces.Compiler.Core.Compilation;

public class IfCommand
{
    public string Id { get; set; } = "";
    public string Condition { get; set; } = "";
    public string IfInnerHtml { get; set; } = "";
    public string ElseInnerHtml { get; set; } = "";
}