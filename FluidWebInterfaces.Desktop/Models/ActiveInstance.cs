using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluidWebInterfaces.Compiler.Core.Compilation;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Models;

public partial class ActiveInstance : ObservableObject
{
    private FileSystemWatcher? _watcher;
    private readonly Func<FileSystemWatcher> _watcherFactory;

    [ObservableProperty]
    private Guid _id;
    
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _active;
    
    [ObservableProperty]
    private string _compiledPath;

    [ObservableProperty]
    private string _sourcePath;

    [ObservableProperty]
    private bool _restartOnError;

    [ObservableProperty]
    private bool _showErrorMessages;

    [ObservableProperty]
    private bool _isShutDown;

    public ActiveInstance(Guid id, string name, bool active, string sourcePath, string compiledPath,
        bool restartOnError, bool showErrorMessages)
    {
        Id = id;
        Name = name;
        Active = active;
        SourcePath = sourcePath;
        CompiledPath = compiledPath;
        RestartOnError = restartOnError;
        ShowErrorMessages = showErrorMessages;

        _watcherFactory = () =>
        {
            var watcher = new FileSystemWatcher(sourcePath)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            watcher.Changed += OnWatcherChange;
            watcher.Created += OnWatcherChange;
            watcher.Deleted += OnWatcherChange;
            watcher.Renamed += OnWatcherChange;
            watcher.Error += OnWatcherError;

            return watcher;
        };

        if (active)
        {
            ResetWatcher();
        }
    }

    [RelayCommand]
    public void Pause()
    {
        if (IsShutDown)
        {
            return;
        }
        
        Active = false;
        _watcher?.Dispose();
        _watcher = null;
        SaveInstance();
    }
    
    [RelayCommand]
    public void Continue()
    {
        if (IsShutDown)
        {
            return;
        }
        if (Active)
        {
            return;
        }

        ResetWatcher();
        Active = true;
        CompileInstance();
        SaveInstance();
    }

    [RelayCommand]
    public void Shutdown()
    {
        if (IsShutDown)
        {
            return;
        }

        IsShutDown = true;
        Active = false;
        
        if (ActiveInstancesViewModel.Current is not null)
        {
            ActiveInstancesViewModel.Current.DeleteInstance(this);
        }
    }

    [RelayCommand]
    private async Task EditNewInstanceAsync()
    {
        if (ActiveInstancesViewModel.Current is not null)
        {
            await ActiveInstancesViewModel.Current.EditInstanceAsync(this);
        }
    }

    [RelayCommand]
    public void OpenFolder(string path)
    {
        string? command = null;
        
        if (OperatingSystem.IsLinux())
        {
            command = "xdg-open";
        }
        if (OperatingSystem.IsWindows())
        {
            command = "explorer.exe";
        }
        if (OperatingSystem.IsMacOS())
        {
            command = "open";
        }

        if (command is not null)
        {
            Process.Start(command, path);
        }
    }

    [RelayCommand]
    public void ToggleRestartOnError()
    {
        RestartOnError = !RestartOnError;
        SaveInstance();
    }

    [RelayCommand]
    public void ToggleShowErrorMessagesCommand()
    {
        ShowErrorMessages = !ShowErrorMessages;
        SaveInstance();
    }

    public void OnWatcherChange(object sender, FileSystemEventArgs e)
    {
        CompileInstance();
    }

    public void CompileInstance()
    {
        if (!Active || IsShutDown)
        {
            return;
        }

        try
        {
            DynamicWebClientCompiler.CompileProject(SourcePath, CompiledPath);
        }
        catch (Exception e)
        {
            Dispatcher.UIThread.Invoke(() => OnCompilationError(e));
        }
    }

    private void OnWatcherError(object sender, ErrorEventArgs e)
    {
        OnCompilationError(e.GetException());
    }

    private void OnCompilationError(Exception e)
    {
        if (ActiveInstancesViewModel.Current is not null && ShowErrorMessages)
        {
            ActiveInstancesViewModel.Current.PushErrorMessage(new Tuple<ActiveInstance, Exception>(this, e));
        }

        if (RestartOnError)
        {
            Continue();
        }
        else
        {
            Pause();
        }
    }

    private void ResetWatcher()
    {
        _watcher = _watcherFactory.Invoke();
    }

    private void SaveInstance()
    {
        if (ActiveInstancesViewModel.Current is not null)
        {
            ActiveInstancesViewModel.Current.SaveInstance(this);
        }
    }
}