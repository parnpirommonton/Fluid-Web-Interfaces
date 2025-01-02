using System.Text;

namespace FluidWebInterfaces.Core.Logging.Providers;

public class ConsoleLoggerProvider : ILoggerProvider
{
    public void Log(LogLevel level, string messageTemplate, object[] parameters)
    {
        var message = MessageTemplateParser.Parse(messageTemplate, parameters);

        switch (level)
        {
            case LogLevel.Trace:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Trace");
                break;
            case LogLevel.Debug:
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Debug");
                break;
            case LogLevel.Information:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("Information");
                break;
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Warning");
                break;
            case LogLevel.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error");
                break;
            default:
                throw new Exception($"{nameof(ConsoleLoggerProvider)} does not support {nameof(LogLevel)} {level}.");
        }
        
        Console.ResetColor();
        Console.Write($": {message} ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"| {DateTime.Now}");
        Console.WriteLine();
    }
}