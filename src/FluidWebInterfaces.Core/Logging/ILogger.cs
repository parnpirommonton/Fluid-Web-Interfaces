namespace FluidWebInterfaces.Core.Logging;

public interface ILogger
{
    void Log(LogLevel level, string messageTemplate, params object[] parameters);
}