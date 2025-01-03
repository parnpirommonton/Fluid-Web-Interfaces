using System;
using System.IO;

namespace FluidWebInterfaces.Desktop;

public static class PersistenceLocations
{
    public static readonly string ApplicationData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FluidWebInterfaces");
    
    static PersistenceLocations()
    {
        CreateDirectory(ApplicationData);
    }

    private static void CreateDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}