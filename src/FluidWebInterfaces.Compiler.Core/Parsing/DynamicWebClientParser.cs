using System.Text;
using FluidWebInterfaces.Compiler.Core.Parsing.Document;
using FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding;
using FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding.Command;

namespace FluidWebInterfaces.Compiler.Core.Parsing;

public static class DynamicWebClientParser
{
    public static DynamicWebClientDocument ParseFile(string sourcePath)
    {
        var content = File.ReadAllText(sourcePath);
        var document = new DynamicWebClientDocument();
        var currentNode = document;
        var withinComment = false;
        
        for (var i = 0; i < content.Length; i++)
        {
            var c = content[i];
            
            if (content.IsNextSequence("<!--", i))
            {
                withinComment = true;
                i += 4;
                continue;
            }

            if (content.IsNextSequence("-->", i))
            {
                withinComment = false;
                i += 3;
                continue;
            }

            if (withinComment)
            {
                continue;
            }
            
            // Process dynamic command.
            if (c is '@')
            {
                i++;
                i = ProcessDynamicCommand(content, i, out var dynamicNode);
                currentNode.Children.Add(dynamicNode);
                continue;
            }

            // Process tag.
            if (c is '<')
            {
                i++;
                i = ProcessTag(content, i, out var node);
                currentNode.Children.Add(node);
                continue;
            }
            
        }

        return document;
    }

    public static int ProcessTag(string content, int i, out HtmlNode node)
    {
        node = new HtmlNode();

        var currentAttributeNameBuilder = new StringBuilder();
        var currentAttributeValueBuilder = new StringBuilder();
        
        var tagNameBuilder = new StringBuilder();
        var findingTagName = true;
        var findingAttributeName = false;
        var insideAttributeValue = false;

        bool isShortEndingTag = false;
        
        // Get tag name and attributes.
        for (; i < content.Length; i++)
        {
            if (content[i] is '>' && !insideAttributeValue)
            {
                i++;
                break;
            }

            if (content.IsNextSequence("/>", i))
            {
                i += 2;
                isShortEndingTag = true;
                break;
            }
            
            // Handle finding tag name.
            if (findingTagName && content[i] is ' ')
            {
                findingTagName = false;
                findingAttributeName = true;
                continue;
            }
            if (findingTagName)
            {
                tagNameBuilder.Append(content[i]);
                continue;
            }
            
            // Handle finding attribute name.
            if (findingAttributeName && content[i] is '=')
            {
                findingAttributeName = false;
                continue;
            }
            if (findingAttributeName)
            {
                currentAttributeNameBuilder.Append(content[i]);
                continue;
            }
            
            // Handle finding attribute value.
            if (!insideAttributeValue && content[i] == '"')
            {
                insideAttributeValue = true;
                continue;
            }
            if (insideAttributeValue && content[i] == '"')
            {
                findingAttributeName = true;
                insideAttributeValue = false;
                node.Attributes.Add(currentAttributeNameBuilder.ToString().RemoveWhitespace(),
                    BindableContent.BuildContentFromAttributeString(currentAttributeValueBuilder.ToString()));
                currentAttributeNameBuilder.Clear();
                currentAttributeValueBuilder.Clear();
                continue;
            }

            if (!insideAttributeValue)
            {
                continue;
            }

            currentAttributeValueBuilder.Append(content[i]);

        }

        node.TagName = tagNameBuilder.ToString().RemoveWhitespace();

        if (isShortEndingTag)
        {
            return i;
        }
        
        //Console.WriteLine(tagName);
        if (node.TagName == "!DOCTYPE")
        {
            return i;
        }

        var textBuilder = new StringBuilder();
        var withinComment = false;
        
        // Otherwise find innerHTML
        for (; i < content.Length; i++)
        {
            
            // Comment ignoring feature.
            
            if (content.IsNextSequence("<!--", i))
            {
                withinComment = true;
                i += 4;
                //Console.WriteLine("Entering comment.");
                continue;
            }

            if (content.IsNextSequence("-->", i))
            {
                withinComment = false;
                i += 3;
                //Console.WriteLine("Exiting comment.");
                continue;
            }

            if (withinComment)
            {
                continue;
            }
            
            // This must be the beginning of the ending tag.
            if (content.IsNextSequence("</", i))
            {
                // Add current text.
                if (textBuilder.Length > 0)
                {
                    var bindableContent = BindableContent.BuildContentFromAttributeString(
                        textBuilder.ToString());
                    foreach (var bindableNode in bindableContent.ContentNodes)
                    {
                        node.Children.Add(bindableNode);
                    }
                    textBuilder.Clear();
                }
                
                i++;
                break;
            }
            
            // Process dynamic command inside innerHTML
            if (content[i] is '@')
            {
                // Add current text.
                if (textBuilder.Length > 0)
                {
                    var bindableContent = BindableContent.BuildContentFromAttributeString(
                        textBuilder.ToString());
                    foreach (var bindableNode in bindableContent.ContentNodes)
                    {
                        node.Children.Add(bindableNode);
                    }
                    textBuilder.Clear();
                }

                i++;
                i = ProcessDynamicCommand(content, i, out var dynamicNode);
                node.Children.Add(dynamicNode);
                continue;
            }

            // Process tag inside innerHTML
            if (content[i] is '<')
            {
                // Add current text.
                if (textBuilder.Length > 0)
                {
                    var bindableContent = BindableContent.BuildContentFromAttributeString(
                        textBuilder.ToString());
                    foreach (var bindableNode in bindableContent.ContentNodes)
                    {
                        node.Children.Add(bindableNode);
                    }
                    textBuilder.Clear();
                }
                
                i++;
                i = ProcessTag(content, i, out var n);
                node.Children.Add(n);
                continue;
            }
            
            textBuilder.Append(content[i]);
        }
        
        // Then skip over the closing tag.
        for (; i < content.Length; i++)
        {
            // This must be the ending of the ending tag.
            if (content[i] is '>')
            {
                i++;
                break;
            }
        }

        return i;
    }

    public static int ProcessDynamicCommand(string content, int i, out DynamicNode node)
    {
        var commandNameBuilder = new StringBuilder();
        
        // First find command name.
        for (; i < content.Length; i++)
        {
            if (content[i] is ' ' or '\t' or '\n')
            {
                i++;
                break;
            }

            commandNameBuilder.Append(content[i]);
        }

        var commandName = commandNameBuilder.ToString();

        switch (commandName)
        {
            case "import":
                i = ImportCommand.ParseFromContent(content, i, out var importCommand);
                node = importCommand;
                break;
            case "render":
                i = RenderCommand.ParseFromContent(content, i, out var renderCommand);
                node = renderCommand;
                break;
            case "foreach":
                i = ForeachCommand.ParseFromContent(content, i, out var foreachCommand);
                node = foreachCommand;
                break;
            case "if":
                i = IfCommand.ParseFromContent(content, i, out var ifCommand);
                node = ifCommand;
                break;
            default:
                throw new Exception($"Command with name {commandName} is not supported.");
        }

        return i;
    }
    
    public static int PopulateBracketContextWithInnerHtml(List<INode> collection, string content, int i)
    {   
        var withinComment = false;
        var textBuilder = new StringBuilder();
        int ifBracketLevel = 0;
        
        for (; i < content.Length; i++)
        {
            if (content[i] is '{')
            {
                ifBracketLevel++;
                if (ifBracketLevel == 1)
                {
                    continue;
                }
            }
            
            if (content[i] is '}')
            {
                ifBracketLevel--;
            }
            
            if (ifBracketLevel == 0)
            {
                if (content[i] is '}')
                {
                    i++;
                    break;
                }
                continue;
            }
            
            // Comment ignoring feature.
            
            if (content.IsNextSequence("<!--", i))
            {
                withinComment = true;
                i += 4;
                //Console.WriteLine("Entering comment.");
                continue;
            }

            if (content.IsNextSequence("-->", i))
            {
                withinComment = false;
                i += 3;
                //Console.WriteLine("Exiting comment.");
                continue;
            }

            if (withinComment)
            {
                continue;
            }
            
            // Process dynamic command inside innerHTML
            if (content[i] is '@')
            {
                // Add current text.
                if (textBuilder.Length > 0)
                {
                    var bindableContent = BindableContent.BuildContentFromAttributeString(
                        textBuilder.ToString());
                    foreach (var bindableNode in bindableContent.ContentNodes)
                    {
                        collection.Add(bindableNode);
                    }
                    textBuilder.Clear();
                }

                i++;
                i = ProcessDynamicCommand(content, i, out var dynamicNode);
                collection.Add(dynamicNode);
                continue;
            }

            // Process tag inside innerHTML
            if (content[i] is '<')
            {
                // Add current text.
                if (textBuilder.Length > 0)
                {
                    var bindableContent = BindableContent.BuildContentFromAttributeString(
                        textBuilder.ToString());
                    foreach (var bindableNode in bindableContent.ContentNodes)
                    {
                        collection.Add(bindableNode);
                    }
                    textBuilder.Clear();
                }
                
                i++;
                i = ProcessTag(content, i, out var n);
                collection.Add(n);
                continue;
            }
            
            textBuilder.Append(content[i]);
        }

        return i;
    }
}