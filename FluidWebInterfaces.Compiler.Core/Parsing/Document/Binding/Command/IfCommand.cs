using System.Text;

namespace FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding.Command;

public class IfCommand : DynamicNode
{
    public string Condition { get; set; } = "";
    public List<INode>? ElseChildren { get; set; }

    public static int ParseFromContent(string content, int i, out IfCommand command)
    {
        var ifCommand = new IfCommand();

        var conditionExpressionBuilder = new StringBuilder();
        
        // First, get the condition of if statement.
        var bracketLevel = 0;
        for (; i < content.Length; i++)
        {
            if (content[i] == '(')
            {
                bracketLevel++;
                
                if (bracketLevel == 1)
                {
                    continue;
                }
            }
            
            if (content[i] == ')')
            {
                bracketLevel--;
                if (bracketLevel == 0)
                {
                    i++;
                    break;
                }
            }

            if (bracketLevel == 0)
            {
                continue;
            }

            conditionExpressionBuilder.Append(content[i]);
        }

        ifCommand.Condition = conditionExpressionBuilder.ToString();
        
        // Find brackets which will contain html and bindable variable syntax.
        i = DynamicWebClientParser.PopulateBracketContextWithInnerHtml(ifCommand.Children, content, i);
        
        // Then see if there are any else/else if statements ahead.
        var currentBranchEditing = ifCommand;
        
        for (; i < content.Length; i++)
        {
            if (content.IsNextSequence("else if", i))
            {
                currentBranchEditing.ElseChildren = [];
                var ifElseBranch = new IfCommand();
                currentBranchEditing.ElseChildren.Add(ifElseBranch);
                currentBranchEditing = ifElseBranch;
                
                // Find else if condition
                var elifConditionBuilder = new StringBuilder();
                var elifBracketLevel = 0;
                for (; i < content.Length; i++)
                {
                    if (content[i] == '(')
                    {
                        elifBracketLevel++;
                
                        if (elifBracketLevel == 1)
                        {
                            continue;
                        }
                    }
            
                    if (content[i] == ')')
                    {
                        elifBracketLevel--;
                        if (elifBracketLevel == 0)
                        {
                            i++;
                            break;
                        }
                    }

                    if (elifBracketLevel == 0)
                    {
                        continue;
                    }

                    elifConditionBuilder.Append(content[i]);
                }

                currentBranchEditing.Condition = elifConditionBuilder.ToString();
                
                // Handle else ... if statement
                i += 7;
                i = DynamicWebClientParser.PopulateBracketContextWithInnerHtml(currentBranchEditing.Children, content, i);
                continue;   
            }
            if (content.IsNextSequence("else", i))
            {
                currentBranchEditing.ElseChildren = [];
                
                // Handle else statement
                i += 4;
                i = DynamicWebClientParser.PopulateBracketContextWithInnerHtml(currentBranchEditing.ElseChildren, content, i);
                continue;
            }
            if (content[i] is ' ' or '\n' or '\t')
            {
                continue;
            }
            
            // Unwanted character was found so stop search.
            i--;
            break;
        }

        command = ifCommand;
        return i;
    }
}