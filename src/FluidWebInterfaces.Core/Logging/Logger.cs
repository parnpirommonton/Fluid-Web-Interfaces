using FluidWebInterfaces.Core.Logging.Providers;

namespace FluidWebInterfaces.Core.Logging;

public class Logger : ILogger
{
    private readonly List<ILoggerProvider> _providers = [];
    private LogLevel _filterLevel = LogLevel.Information;
    
    public void Log(LogLevel level, string messageTemplate, params object[] parameters)
    {
        if (_filterLevel > level)
        {
            return;
        }
        
        foreach (var provider in _providers)
        {
            provider.Log(level, messageTemplate, parameters);
        }
    }

    public void AddProvider(ILoggerProvider provider)
    {
        _providers.Add(provider);
    }

    public void AddConsoleLogging()
    {
        AddProvider(new ConsoleLoggerProvider());
    }

    public void AddFileLogging(string logPath)
    {
        AddProvider(new FileLoggerProvider(logPath));
    }

    public void FilterToLevel(LogLevel logLevel)
    {
        _filterLevel = logLevel;
    }
}