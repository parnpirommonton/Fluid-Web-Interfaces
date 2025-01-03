using System;
using System.IO;
using FluidWebInterfaces.Core.Logging;

namespace FluidWebInterfaces.Desktop;

public static class PersistenceLocations
{
    public static readonly string ApplicationData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FluidWebInterfaces");

    public static readonly string LoggingData = Path.Combine(ApplicationData, "logging");
    public static readonly string LogOutputFile = Path.Combine(LoggingData, "log.txt");
    public static readonly string LogLevelFile = Path.Combine(LoggingData, "log-level.txt");
    
    static PersistenceLocations()
    {
        CreateDirectory(ApplicationData);
        CreateDirectory(LoggingData);
        CreateFile(LogOutputFile, "");
        CreateFile(LogLevelFile, ((int)LogLevel.Information).ToString());
    }

    private static void CreateDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static void CreateFile(string file, string defaultContent)
    {
        if (!File.Exists(file))
        {
            File.WriteAllText(file, defaultContent);
        }
    }
}