using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.Mapping;

public static class InstanceMapping
{
    public static ActiveInstance ActivateInstance(this Instance instance)
    {
        return new ActiveInstance(
            instance.Id,
            instance.Name,
            instance.Active,
            instance.SourcePath,
            instance.CompiledPath,
            instance.RestartOnError,
            instance.ShowErrorMessages);
    }
    public static Instance ToInstance(this ActiveInstance activeInstance)
    {
        return new Instance
        {
            Id = activeInstance.Id,
            Name = activeInstance.Name,
            Active = activeInstance.Active,
            SourcePath = activeInstance.SourcePath,
            CompiledPath = activeInstance.CompiledPath,
            RestartOnError = activeInstance.RestartOnError,
            ShowErrorMessages = activeInstance.ShowErrorMessages
        };
    }

    public static void MapFromInstance(this ActiveInstance activeInstance, Instance instance)
    {
        activeInstance.Name = instance.Name;
        activeInstance.Active = instance.Active;
        activeInstance.SourcePath = instance.SourcePath;
        activeInstance.CompiledPath = instance.CompiledPath;
        activeInstance.RestartOnError = instance.RestartOnError;
        activeInstance.ShowErrorMessages = instance.ShowErrorMessages;
    }
}