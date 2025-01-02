using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluidWebInterfaces.Desktop.Models;
using FluidWebInterfaces.Desktop.Services;

namespace FluidWebInterfaces.Desktop.ViewModels;

public partial class CreateNewInstanceViewModel : ViewModelBase
{
    private readonly IInstanceService _instanceService;
    
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

    public CreateNewInstanceViewModel(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    public bool TryCreateInstance()
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

        _instanceService.Create(
            new Instance
            {
                Id = Guid.NewGuid(),
                Name = Name,
                Active = false,
                SourcePath = SourceFolder,
                CompiledPath = OutputFolder,
                RestartOnError = RestartOnError,
                ShowErrorMessages = ShowErrorMessages
            });

        return true;
    }
}