using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FluidWebInterfaces.Desktop.Models;

namespace FluidWebInterfaces.Desktop.ViewModels;

public partial class ActiveInstancesErrorViewModel : ViewModelBase
{
    [ObservableProperty]
    private ActiveInstance? _activeInstance;

    [ObservableProperty]
    private string? _exceptionText;
    
    public override void Initialize(object? parameter = null)
    {
        var tuple = (Tuple<ActiveInstance, Exception>?)parameter;

        if (tuple is null)
        {
            throw new Exception($"{nameof(ActiveInstancesErrorViewModel)} requires a parameter of type {nameof(Tuple<ActiveInstance, Exception>)}.");
        }

        ActiveInstance = tuple.Item1;
        ExceptionText = tuple.Item2.Message;
    }
}