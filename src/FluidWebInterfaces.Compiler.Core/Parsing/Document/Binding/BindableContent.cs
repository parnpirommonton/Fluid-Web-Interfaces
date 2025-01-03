using System.Text;

namespace FluidWebInterfaces.Compiler.Core.Parsing.Document.Binding;

public class BindableContent
{
     public List<BindableContentNode> ContentNodes = [];

     public static BindableContent BuildContentFromAttributeString(string attributeExpression)
     {
          var content = new BindableContent();
          
          var e = attributeExpression;
          var withinBinding = false;

          var bindingBuilder = new StringBuilder();
          var textBuilder = new StringBuilder();
          
          for (var i = 0; i < attributeExpression.Length; i++)
          {
               if (e[i] is '{')
               {
                    withinBinding = true;
                    if (textBuilder.Length > 0)
                    {
                         content.ContentNodes.Add(
                              new BindableContentNode
                              {
                                   ContentType = NodeContentType.Text,
                                   StringValue = textBuilder.ToString()
                              });
                         textBuilder.Clear();
                    }
                    continue;
               }
               if (e[i] is '}')
               {
                    withinBinding = false;
                    content.ContentNodes.Add(
                         new BindableContentNode
                         {
                              ContentType = NodeContentType.Variable,
                              VariableReference = bindingBuilder.ToString()
                         });
                    bindingBuilder.Clear();
                    continue;
               }

               if (withinBinding)
               {
                    bindingBuilder.Append(e[i]);
                    continue;
               }

               textBuilder.Append(e[i]);
          }

          if (textBuilder.Length > 0)
          {
               content.ContentNodes.Add(
                    new BindableContentNode
                    {
                         ContentType = NodeContentType.Text,
                         StringValue = textBuilder.ToString()
                    });
               textBuilder.Clear();
          }

          return content;
     }
}