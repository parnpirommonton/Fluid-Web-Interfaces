namespace FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding.Command;

public class RenderCommand : DynamicNode
{
    public string Source { get; set; } = "";

    public static int ParseFromContent(string content, int i, out RenderCommand command)
    {
        var renderCommand = new RenderCommand();

        // Get source of import.
        var findingRenderSource = false;
                
        for (; i < content.Length; i++)
        {
            if (content[i] is '"' && !findingRenderSource)
            {
                findingRenderSource = true;
                continue;
            }

            if (content[i] is '"' && findingRenderSource)
            {
                i++;
                break;
            }

            renderCommand.Source += content[i];
        }

        command = renderCommand;
        return i;
    }
}