using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using FluidWebInterfaces.Desktop.Models;
using FluidWebInterfaces.Desktop.Services;

namespace FluidWebInterfaces.Desktop.ViewModels;

public partial class EditActiveInstanceViewModel : ViewModelBase
{
    private readonly IInstanceService _instanceService;

    [ObservableProperty]
    private ActiveInstance? _currentInstance;
    
    [ObservableProperty]
    private string _name = "";
    
    [ObservableProperty]
    private string _sourceFolder = "";
    
    [ObservableProperty]
    private string _outputFolder = "";
    
    [ObservableProperty]
    private string _issueMessage = "";

    [ObservableProperty]
    private bool _restartOnError;

    [ObservableProperty]
    private bool _showErrorMessages;

    [ObservableProperty]
    private string? _title;

    public EditActiveInstanceViewModel(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    public override void Initialize(object? parameter = null)
    {
        var instance = (ActiveInstance?)parameter;

        if (instance is null)
        {
            throw new Exception(
                $"Parameter {nameof(instance)} must be provided to view model {nameof(EditActiveInstanceViewModel)} when initializing.");
        }

        CurrentInstance = instance;

        Name = CurrentInstance.Name;
        SourceFolder = CurrentInstance.SourcePath;
        OutputFolder = CurrentInstance.CompiledPath;
        RestartOnError = CurrentInstance.RestartOnError;
        ShowErrorMessages = CurrentInstance.ShowErrorMessages;

        Title = "Editing Active Instance";
    }

    public bool TryUpdateInstance()
    {
        IssueMessage = "";
        
        if (Name is "")
        {
            IssueMessage = "Please provide a name for the instance.";
            return false;
        }
        if (!Directory.Exists(SourceFolder))
        {
            IssueMessage = "The source folder does not exist.";   
            return false;
        }
        if (!Directory.Exists(OutputFolder))
        {
            IssueMessage = "The output folder does not exist.";
            return false;
        }

        _instanceService.Update(new Instance
        {
            Id = CurrentInstance!.Id,
            Name = Name,
            Active = CurrentInstance.Active,
            SourcePath = SourceFolder,
            CompiledPath = OutputFolder,
            RestartOnError = RestartOnError,
            ShowErrorMessages = ShowErrorMessages
        });

        return true;
    }
}