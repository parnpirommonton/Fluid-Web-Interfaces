namespace FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding.Command;

public class ImportCommand : DynamicNode
{
    public string TagAlias { get; set; } = "";
    public string Source { get; set; } = "";

    public static int ParseFromContent(string content, int i, out ImportCommand command)
    {
        var importCommand = new ImportCommand();

        // Get source of import.
        var findingSource = false;
                
        for (; i < content.Length; i++)
        {
            if (content[i] is '"' && !findingSource)
            {
                findingSource = true;
                continue;
            }

            if (content[i] is '"' && findingSource)
            {
                i++;
                break;
            }

            importCommand.Source += content[i];
        }
                
        // Skip over 'as' keyword.
        for (; i < content.Length; i++)
        {
            if (content.IsNextSequence("as", i, false))
            {
                i += 2;
                break;
            }
        }
                
        // Get alias of import.
        var findingAlias = false;
        for (; i < content.Length; i++)
        {
            if (!findingAlias && content[i] is ' ' or '\t' or '\n')
            {
                continue;
            }
                    
            if (content[i] is ' ' or '\t' or '\n')
            {
                break;
            }

            findingAlias = true;

            importCommand.TagAlias += content[i];
        }

        command = importCommand;
        return i;
    }
}