using FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding;

namespace FluidWebInterfaces.Compiler.Core.Parsing.Document;

public class HtmlNode : DocumentNode
{
    public string TagName { get; set; } = "";
    public Dictionary<string, BindableContent> Attributes { get; set; } = [];
}