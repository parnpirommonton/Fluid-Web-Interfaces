namespace FluidWebInterfaces.Core.Logging.Providers;

public class FileLoggerProvider : ILoggerProvider
{
    private string _logPath;
    public FileLoggerProvider(string logPath)
    {
        _logPath = logPath;

        if (!File.Exists(_logPath))
        {
            File.WriteAllText(_logPath, "");
        }
    }
    
    public void Log(LogLevel level, string messageTemplate, object[] parameters)
    {
        var message = MessageTemplateParser.Parse(messageTemplate, parameters);
        
        using var writer = File.AppendText(_logPath);
        writer.WriteLine($"{level.ToString()}: {message} | {DateTime.Now}");
        writer.Close();
    }
}