using System;

namespace FluidWebInterfaces.Desktop.Models;

public class Instance
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required bool Active { get; set; }
    public required string CompiledPath { get; set; }
    public required string SourcePath { get; set; }
    public required bool RestartOnError { get; set; }
    public required bool ShowErrorMessages { get; set; }
}