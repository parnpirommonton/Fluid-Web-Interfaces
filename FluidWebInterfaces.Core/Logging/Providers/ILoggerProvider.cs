namespace FluidWebInterfaces.Core.Logging.Providers;

public interface ILoggerProvider
{
    void Log(LogLevel level, string messageTemplate, object[] parameters);
}