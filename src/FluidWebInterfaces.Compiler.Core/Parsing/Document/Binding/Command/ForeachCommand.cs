using System.Text;

namespace FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding.Command;

public class ForeachCommand : DynamicNode
{
    public string ItemDeclarationType { get; set; } = "";
    public string ItemDeclaration { get; set; } = "";
    public string CollectionDeclaration { get; set; } = "";

    public static int ParseFromContent(string content, int i, out ForeachCommand command)
    {
        var foreachCommand = new ForeachCommand();

        // First get to the start of the foreach bracket expression.
        for (; i < content.Length; i++)
        {
            if (content[i] is '(')
            {
                i++;
                break;
            }
            
            continue;
        }
        
        // Next get the variable declaration type within the bracket expression.
        bool foundDeclarationType = false;
        var declarationTypeBuilder = new StringBuilder();
        for (; i < content.Length; i++)
        {
            if (content[i] is ' ' or '\t' or '\n' && !foundDeclarationType)
            {
                continue;
            }
            
            if (content[i] is ' ' or '\t' or '\n')
            {
                break;
            }

            if (!foundDeclarationType)
            {
                foundDeclarationType = true;
            }

            declarationTypeBuilder.Append(content[i]);
        }

        var declarationType = declarationTypeBuilder.ToString();
        if (declarationType is not "let" and not "const" and not "var")
        {
            throw new Exception($"Invalid declaration type at index {i}.");
        }

        foreachCommand.ItemDeclarationType = declarationType;
        
        // Then get the variable declaration within the bracket expression.
        bool foundDeclaration = false;
        var declarationBuilder = new StringBuilder();
        for (; i < content.Length; i++)
        {
            if (content[i] is ' ' or '\t' or '\n' && !foundDeclaration)
            {
                continue;
            }
            
            if (content[i] is ' ' or '\t' or '\n')
            {
                break;
            }

            if (!foundDeclaration)
            {
                foundDeclaration = true;
            }

            declarationBuilder.Append(content[i]);
        }

        var declaration = declarationBuilder.ToString();
        foreachCommand.ItemDeclaration = declaration;
        
        // Skip over 'in' keyword and spaces.
        for (; i < content.Length; i++)
        {
            if (content.IsNextSequence("in", i, false))
            {
                i += 2;
                break;
            }
        }
        for (; i < content.Length; i++)
        {
            if (content[i] is ' ' or '\t' or '\n')
            {
                continue;
            }
            break;
        }
        
        // Finally get the collection expression and end of the entire foreach bracket expression.
        // We start off with bracket level 1 since we are already inside the bracket expression.
        int bracketLevel = 1;
        StringBuilder collectionExpressionBuilder = new();
        for (; i < content.Length; i++)
        {
            if (content[i] is '(')
            {
                bracketLevel++;
                continue;
            }
            if (content[i] is ')')
            {
                bracketLevel--;

                if (bracketLevel == 0)
                {
                    i++;
                    break;
                }
                
                continue;
            }

            collectionExpressionBuilder.Append(content[i]);
        }

        foreachCommand.CollectionDeclaration = collectionExpressionBuilder.ToString();
        
        // Now get the body of the foreach
        i = DynamicWebClientParser.PopulateBracketContextWithInnerHtml(foreachCommand.Children, content, i);

        command = foreachCommand;
        return i;
    }
}