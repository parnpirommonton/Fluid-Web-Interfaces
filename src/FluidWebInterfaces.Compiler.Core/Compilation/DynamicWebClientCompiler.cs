using FluidWebInterfaces.Compiler.Core.Parsing;

namespace FluidWebInterfaces.Compiler.Core.Compilation;

public static class DynamicWebClientCompiler
{
    public static void CompileProject(string srcDirectory, string outputDirectory)
    {
        // Clear the output directory before compiling.
        foreach (var directory in Directory.EnumerateDirectories(outputDirectory))
        {
            Directory.Delete(directory, true);
        }
        foreach (var file in Directory.EnumerateFiles(outputDirectory))
        {
            File.Delete(file);
        }
        
        CompileDirectory(srcDirectory, outputDirectory, srcDirectory);
    }

    public static void CompileDirectory(string srcDirectory, string outputDirectory, string currentDirectory)
    {
        foreach (var file in Directory.EnumerateFiles(currentDirectory))
        {
            var outputDirectoryPath = Path.Join(outputDirectory, currentDirectory[srcDirectory.Length..]);
            DynamicWebClientFileCompiler.CompileFile(outputDirectoryPath, file);
        }

        foreach (var directory in Directory.EnumerateDirectories(currentDirectory))
        {
            CompileDirectory(srcDirectory, outputDirectory, directory);
        }
    }
}