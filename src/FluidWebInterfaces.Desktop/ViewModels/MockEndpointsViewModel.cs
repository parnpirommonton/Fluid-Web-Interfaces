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

public partial class MockEndpointsViewModel : ViewModelBase
{
    public static MockEndpointsViewModel? Current { get; private set; }

    [ObservableProperty]
    private string _filterText = "";

    [ObservableProperty]
    private ObservableCollection<ActiveMockEndpoint> _mockEndpoints = [];

    [ObservableProperty]
    private ObservableCollection<ActiveMockEndpoint> _filteredMockEndpoints = [];

    private readonly IMockEndpointService _mockEndpointService;
    private readonly IAsyncNavigationService _dialogService;
    private readonly HostingService _hostingService;

    public MockEndpointsViewModel(IMockEndpointService mockEndpointService,
        IAsyncNavigationService dialogService, HostingService hostingService)
    {
        _mockEndpointService = mockEndpointService;
        _dialogService = dialogService;
        _hostingService = hostingService;
        Current = this;
    }

    public override void Initialize(object? parameter = null)
    {
        UpdateActiveMockEndpoints();
        RefreshFilteredActiveMockEndpoints();
    }

    public void DeleteEndpoint(ActiveMockEndpoint endpoint)
    {
        _mockEndpointService.DeleteById(endpoint.Id);
        MockEndpoints.Remove(endpoint);
        RefreshFilteredActiveMockEndpoints();
    }

    [RelayCommand]
    public void StopAllMockEndpoints()
    {
        foreach (var endpoint in MockEndpoints)
        {
            endpoint.Pause();
        }
    }

    [RelayCommand]
    public void RestartAllMockEndpoints()
    {
        StopAllMockEndpoints();
        foreach (var endpoint in MockEndpoints)
        {
            endpoint.Continue();
        }
    }

    public void SaveMockEndpoint(ActiveMockEndpoint endpoint)
    {
        _mockEndpointService.Update(endpoint.ToMockEndpoint());
    }

    public async Task EditMockEndpointAsync(ActiveMockEndpoint endpoint)
    {
        await _dialogService.NavigateToViewOfModelAsync<EditMockEndpointViewModel>(endpoint);
    }

    [RelayCommand]
    private async Task CreateInstanceAsync()
    {
        await _dialogService.NavigateToViewOfModelAsync<CreateNewMockEndpointViewModel>();
    }

    public void RefreshFilteredActiveMockEndpoints()
    {
        FilteredMockEndpoints = new ObservableCollection<ActiveMockEndpoint>(
            MockEndpoints.Where(instance => instance.Name.ToLower().Contains(FilterText.ToLower())));
    }

    public void RefreshViewModel()
    {
        Initialize();
    }

    public void UpdateActiveMockEndpoints()
    {
        var endpoints = _mockEndpointService.GetAll().ToArray();

        List<ActiveMockEndpoint> toDispose = [];
        
        foreach (var endpoint in MockEndpoints)
        {
            var existingEndpoint = endpoints.SingleOrDefault(
                instance => instance.Id == endpoint.Id);

            if (existingEndpoint is null)
            {
                toDispose.Add(endpoint);
            }
        }

        foreach (var endpoint in toDispose)
        {
            endpoint.Shutdown();
        }

        foreach (var endpoint in endpoints)
        {
            var existingEndpoint = MockEndpoints.SingleOrDefault(
                activeInstance => activeInstance.Id == endpoint.Id);
            
            if (existingEndpoint is null)
            {
                var activeEndpoint = endpoint.ActivateMockEndpoint(_hostingService);
                MockEndpoints.Add(activeEndpoint);
            }
            else
            {
                existingEndpoint.MapFromMockEndpoint(endpoint);
            }
        }
    }
}