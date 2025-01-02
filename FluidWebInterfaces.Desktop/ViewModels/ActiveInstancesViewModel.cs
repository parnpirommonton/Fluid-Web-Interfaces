using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluidWebInterfaces.Desktop.Mapping;
using FluidWebInterfaces.Desktop.Models;
using FluidWebInterfaces.Desktop.Services;

namespace FluidWebInterfaces.Desktop.ViewModels;

public partial class ActiveInstancesViewModel : ViewModelBase
{
    public static ActiveInstancesViewModel? Current { get; private set; }

    private readonly IAsyncNavigationService _dialogService;
    private readonly IInstanceService _instanceService;

    [ObservableProperty]
    private ObservableCollection<ActiveInstance> _activeInstances = [];

    [ObservableProperty]
    private ObservableCollection<ActiveInstance> _filteredActiveInstances = [];

    [ObservableProperty]
    private string _filterText = "";

    public ActiveInstancesViewModel(IInstanceService instanceService,
        IAsyncNavigationService dialogService)
    {
        _instanceService = instanceService;
        _dialogService = dialogService;
        Current = this;
    }

    public override void Initialize(object? parameter = null)
    {
        UpdateActiveInstances();
        RefreshFilteredActiveInstances();
    }

    public void DeleteInstance(ActiveInstance instance)
    {
        _instanceService.DeleteById(instance.Id);
        ActiveInstances.Remove(instance);
        RefreshFilteredActiveInstances();
    }

    public void SaveInstance(ActiveInstance instance)
    {
        _instanceService.Update(instance.ToInstance());
    }

    public void PushErrorMessage(Tuple<ActiveInstance, Exception> tuple)
    {
        _dialogService.NavigateToViewOfModelAsync<ActiveInstancesErrorViewModel>(tuple);
    }

    [RelayCommand]
    public void StopAllInstances()
    {
        foreach (var instance in ActiveInstances)
        {
            instance.Pause();
        }
    }

    [RelayCommand]
    public void RestartAllInstances()
    {
        StopAllInstances();
        foreach (var instance in ActiveInstances)
        {
            instance.Continue();
        }
    }

    [RelayCommand]
    private async Task CreateNewInstanceAsync()
    {
        await _dialogService.NavigateToViewOfModelAsync<CreateNewInstanceViewModel>();
    }

    public async Task EditInstanceAsync(ActiveInstance instance)
    {
        await _dialogService.NavigateToViewOfModelAsync<EditActiveInstanceViewModel>(
            instance);
    }

    public void RefreshViewModel()
    {
        Initialize();
    }

    public void RefreshFilteredActiveInstances()
    {
        FilteredActiveInstances = new ObservableCollection<ActiveInstance>(
            ActiveInstances.Where(instance => instance.Name.ToLower().Contains(FilterText.ToLower())));
    }

    public void UpdateActiveInstances()
    {
        var instances = _instanceService.GetAll().ToArray();

        List<ActiveInstance> toDispose = [];
        
        foreach (var activeInstance in ActiveInstances)
        {
            var existingInstance = instances.SingleOrDefault(
                instance => instance.Id == activeInstance.Id);

            if (existingInstance is null)
            {
                toDispose.Add(activeInstance);
            }
        }

        foreach (var activeInstance in toDispose)
        {
            activeInstance.Shutdown();
        }

        foreach (var instance in instances)
        {
            var existingInstance = ActiveInstances.SingleOrDefault(
                activeInstance => activeInstance.Id == instance.Id);
            
            if (existingInstance is null)
            {
                ActiveInstances.Add(instance.ActivateInstance());
            }
            else
            {
                existingInstance.MapFromInstance(instance);
            }
        }
    }
}