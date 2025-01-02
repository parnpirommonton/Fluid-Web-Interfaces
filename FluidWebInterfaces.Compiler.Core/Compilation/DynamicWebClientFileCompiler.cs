using System.Text;
using FluidWebInterfaces.Compiler.Core.Parsing;
using FluidWebInterfaces.Compiler.Core.Parsing.Document;
using FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding;
using FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding.Command;

namespace FluidWebInterfaces.Compiler.Core.Compilation;

public class DynamicWebClientFileCompiler
{
    public static void CompileFile(string outputDirectoryPath, string currentFilePath)
    {
        var fileName = Path.GetFileName(currentFilePath);

        if (fileName[0] is '_')
        {
            return;
        }

        if (! (Path.GetExtension(currentFilePath) is ".htm" or ".html"))
        {
            Console.WriteLine(Path.Join(outputDirectoryPath, fileName));
            File.WriteAllText(Path.Join(outputDirectoryPath, fileName), File.ReadAllText(currentFilePath));
            return;
        }
        
        if (!Directory.Exists(outputDirectoryPath))
        {
            Directory.CreateDirectory(outputDirectoryPath);
        }

        var contentTree = DynamicWebClientParser.ParseFile(currentFilePath);
        
        contentTree.Children.Add(
            new BindableContentNode
            {
                ContentType = NodeContentType.Text,
                StringValue = "\n"
            });
        contentTree.Children.Add(
            new HtmlNode
            {
                TagName = "script",
                Children = {
                    new BindableContentNode
                    {
                        ContentType = NodeContentType.Text,
                        StringValue = "onStateChanged();"
                    }
                }
            });
        
        List<BoundProperty> boundProperties = [];
        List<IfCommand> ifCommands = [];
        List<CustomTagDeclaration> customTags = [];
        var compiledContent = CompileCurrentNode(contentTree, boundProperties, ifCommands, customTags, [], fileName, currentFilePath);

        CreatePropertyFile(fileName, outputDirectoryPath, boundProperties, ifCommands);
        
        File.WriteAllText(Path.Join(outputDirectoryPath, fileName), compiledContent);
    }

    public static void CreatePropertyFile(string fileName,
        string outputDirectoryPath, List<BoundProperty> properties, List<IfCommand> ifCommands)
    {
        var propertiesSplit = string.Concat(properties.Select(property =>
            $"new Property(\"{property.Id}\", \"{property.Attribute}\", () => {property.Value}), "));
        var ifCommandsSplit = string.Concat(ifCommands.Select(command =>
            $"new IfCommand(\"{command.Id}\", () => {command.Condition}, `{command.IfInnerHtml}`, `{command.ElseInnerHtml}`), "));

        string content = JavaScriptBindingFile.Content(propertiesSplit, ifCommandsSplit);
        
        File.WriteAllText(Path.Combine(outputDirectoryPath, $"{fileName}_binding.js"), content);
    }

    public static string CompileCurrentNode(INode currentNode,
        List<BoundProperty> boundProperties, List<IfCommand> ifCommands, List<CustomTagDeclaration> customTags,
        Dictionary<string, string> parentAliasScope, string fileName, string currentFilePath)
    {
        // Clone the alias scope.
        Dictionary<string, string> aliasScope = [];
        foreach (var (key, value) in parentAliasScope)
        {
            aliasScope[key] = value;
        }
        
        StringBuilder builder = new();
        
        if (currentNode is DynamicWebClientDocument document)
        {
            foreach (var child in document.Children)
            {
                builder.Append(CompileCurrentNode(child, boundProperties, ifCommands, customTags, aliasScope, fileName, currentFilePath));
            }
        }
        if (currentNode is HtmlNode htmlNode)
        {
            if (htmlNode.TagName is "!DOCTYPE")
            {
                builder.Append("<!DOCTYPE html>\n");
                return builder.ToString();
            }

            var customTagDeclaration = customTags.SingleOrDefault(declaration => declaration.TagName == htmlNode.TagName);

            if (customTagDeclaration is not null)
            {
                // Handle custom tag.
            }
            
            builder.Append($"<{htmlNode.TagName} ");

            List<Guid> elementBoundProperties = [];
            
            // Add attributes.
            foreach (var attribute in htmlNode.Attributes)
            {
                bool isBoundValue = false;
                StringBuilder attributeBuilder = new();
                
                foreach (var content in attribute.Value.ContentNodes)
                {
                    if (content.ContentType == NodeContentType.Variable)
                    {
                        isBoundValue = true;
                        attributeBuilder.Append($"${{{content.VariableReference}}}");
                    }
                    if (content.ContentType == NodeContentType.Text)
                    {
                        attributeBuilder.Append(content.StringValue);
                    }
                }

                if (isBoundValue)
                {
                    var boundId = Guid.NewGuid();
                    var boundProperty = new BoundProperty(boundId, $"`{attributeBuilder}`", attribute.Key);
                    boundProperties.Add(boundProperty);
                    elementBoundProperties.Add(boundId);
                }
                else
                {
                    builder.Append($"{attribute.Key}=\"{attributeBuilder.ToString()}\" ");
                }
            }
            
            // Get innerHtml.
            StringBuilder innerHtmlBuilder = new();

            if (htmlNode.TagName is "title")
            {
                StringBuilder expressionBuilder = new();
                
                expressionBuilder.Append('`');
                foreach (var innerHtmlNode in htmlNode.Children)
                {
                    if (innerHtmlNode is BindableContentNode bindableContentNode)
                    {
                        if (bindableContentNode.ContentType is NodeContentType.Text)
                        {
                            expressionBuilder.Append(bindableContentNode.StringValue);
                        }
                        if (bindableContentNode.ContentType is NodeContentType.Variable)
                        {
                            expressionBuilder.Append($"${{{bindableContentNode.VariableReference}}}");
                        }
                    }
                    else
                    {
                        expressionBuilder.Append(CompileCurrentNode(innerHtmlNode, boundProperties, ifCommands, customTags,
                            aliasScope, fileName, currentFilePath));
                    }
                }
                expressionBuilder.Append('`');
                
                var propertyId = Guid.NewGuid();
                var property = new BoundProperty(propertyId, expressionBuilder.ToString(), "innerHTML");
                boundProperties.Add(property);
                elementBoundProperties.Add(propertyId);
            }
            else
            {
                // If the tag is a head, add the binding file.
                if (htmlNode.TagName is "head")
                {
                    htmlNode.Children.Insert(0, new HtmlNode
                    {
                        TagName = "script",
                        Attributes = {
                            {
                                "src",
                                new BindableContent
                                {
                                    ContentNodes = [
                                        new BindableContentNode
                                        {
                                            ContentType = NodeContentType.Text,
                                            StringValue = $"./{fileName}_binding.js"
                                        }
                                    ]
                                }
                            }
                        }
                    });
                    htmlNode.Children.Insert(0, new BindableContentNode
                    {
                        ContentType = NodeContentType.Text,
                        StringValue = "\n\t\t"
                    });
                }
                
                foreach (var innerHtmlNode in htmlNode.Children)
                {
                    if (innerHtmlNode is BindableContentNode bindableContent)
                    {
                        if (bindableContent.ContentType is NodeContentType.Text)
                        {
                            innerHtmlBuilder.Append(bindableContent.StringValue);
                        }
                        if (bindableContent.ContentType is NodeContentType.Variable)
                        {
                            var propertyId = Guid.NewGuid();
                            var property = new BoundProperty(propertyId, bindableContent.VariableReference, "innerHTML");
                            boundProperties.Add(property);
                            innerHtmlBuilder.Append($"<span data-bound-properties=\"{propertyId}\"></span>");
                        }
                    }
                    else
                    {
                        innerHtmlBuilder.Append(CompileCurrentNode(
                            innerHtmlNode, boundProperties, ifCommands, customTags, aliasScope, fileName, currentFilePath));
                    }
                }
            }
            
            // Add properties
            if (elementBoundProperties.Count > 0)
            {
                builder.Append("data-bound-properties=\"");
                foreach (var id in elementBoundProperties)
                {
                    builder.Append($"{id} ");
                }
                builder.Append('\"');
            }
            
            // Close the tag.
            builder.Append('>');
            
            // Add innerHtml to the tag.
            builder.Append(innerHtmlBuilder);
            
            // Close the pair.
            builder.Append($"</{htmlNode.TagName}>");
        }
        if (currentNode is BindableContentNode bindableNode)
        {
            if (bindableNode.ContentType == NodeContentType.Text)
            {
                builder.Append(bindableNode.StringValue);
            }
            if (bindableNode.ContentType == NodeContentType.Variable)
            {
                var propertyId = Guid.NewGuid();
                var property = new BoundProperty(propertyId, bindableNode.VariableReference, "innerHTML");
                boundProperties.Add(property);
                builder.Append($"<span data-bound-properties=\"{propertyId}\"></span>");
            }
        }
        if (currentNode is Core.Parsing.Document.Binding.Command.IfCommand ifCommand)
        {
            var ifCommandId = Guid.NewGuid();
            builder.Append($"<div data-if-id=\"{ifCommandId}\"></div>");
            var ifCommandInstance = new IfCommand();
            ifCommands.Add(ifCommandInstance);

            var ifInnerHtml = new StringBuilder();
            foreach (var node in ifCommand.Children)
            {
                ifInnerHtml.Append(CompileCurrentNode(node, boundProperties, ifCommands, customTags, aliasScope, fileName, currentFilePath));
            }
            var elseInnerHtml = new StringBuilder();
            if (ifCommand.ElseChildren is not null)
            {
                foreach (var node in ifCommand.ElseChildren)
                {
                    elseInnerHtml.Append(CompileCurrentNode(node, boundProperties, ifCommands, customTags, aliasScope, fileName, currentFilePath));
                }
            }

            ifCommandInstance.Id = ifCommandId.ToString();
            ifCommandInstance.Condition = ifCommand.Condition;
            ifCommandInstance.IfInnerHtml = ifInnerHtml.ToString();
            ifCommandInstance.ElseInnerHtml = elseInnerHtml.ToString();
        }
        if (currentNode is RenderCommand renderCommand)
        {
            var resourceUri = Path.GetFullPath(Path.Combine(Directory.GetParent(currentFilePath)!.FullName, renderCommand.Source));

            var contentTree = DynamicWebClientParser.ParseFile(resourceUri);
            var content = CompileCurrentNode(contentTree, boundProperties, ifCommands, customTags, aliasScope, Path.GetFileName(resourceUri), resourceUri);
            builder.Append(content);
        }
        if (currentNode is ImportCommand importCommand)
        {
            var resourceUri = Path.GetFullPath(Path.Combine(Directory.GetParent(currentFilePath)!.FullName, importCommand.Source));
            var declaration = new CustomTagDeclaration(importCommand.TagAlias, resourceUri);
            
            customTags.Add(declaration);
        }

        return builder.ToString();
    }
}