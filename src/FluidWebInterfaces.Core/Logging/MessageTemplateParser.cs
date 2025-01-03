using System.Text;

namespace FluidWebInterfaces.Core.Logging;

public static class MessageTemplateParser
{
    public static string Parse(string messageTemplate, object[] parameters)
    {
        Dictionary<string, string> embeddedValues = [];

        StringBuilder messageBuilder = new();
        StringBuilder nameBuilder = new();
        bool findingEmbeddedValue = false;
        
        for (int i = 0; i < messageTemplate.Length; i++)
        {
            if (messageTemplate[i] is '{')
            {
                findingEmbeddedValue = true;
                continue;
            }
            if (messageTemplate[i] is '}')
            {
                var name = nameBuilder.ToString();

                if (embeddedValues.TryGetValue(name, out var value))
                {
                    messageBuilder.Append(value);
                }
                else
                {
                    embeddedValues[name] = parameters[embeddedValues.Count].ToString()!;
                    messageBuilder.Append(embeddedValues[name]);
                }
                
                nameBuilder.Clear();
                findingEmbeddedValue = false;
                continue;
            }

            if (findingEmbeddedValue)
            {
                nameBuilder.Append(messageTemplate[i]);
                continue;
            }

            messageBuilder.Append(messageTemplate[i]);
        }

        return messageBuilder.ToString();
    }
}