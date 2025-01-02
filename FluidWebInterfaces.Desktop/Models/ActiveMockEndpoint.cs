using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluidWebInterfaces.Desktop.Services;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Models;

public partial class ActiveMockEndpoint : ObservableObject
{
    private readonly HostingService _hostingService;
    
    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private string _name;
    
    [ObservableProperty]
    private int _port;

    [ObservableProperty]
    private string _route;

    [ObservableProperty]
    private bool _active;

    [ObservableProperty]
    private string _responseText;

    [ObservableProperty]
    private string _responseType;

    private bool _isShutdown;

    public ActiveMockEndpoint(HostingService hostingService, Guid id, string name, int port, string route, bool active, string responseText, string responseType)
    {
        _hostingService = hostingService;
        
        Id = id;
        Name = name;
        Port = port;
        Route = route;
        ResponseText = responseText;
        ResponseType = responseType;
        Active = active;

        _isShutdown = false;

        if (Active)
        {
            _hostingService.AddEndpoint(this);
        }
    }

    [RelayCommand]
    public void Shutdown()
    {
        Pause();
        _isShutdown = true;

        if (MockEndpointsViewModel.Current is not null)
        {
            MockEndpointsViewModel.Current.DeleteEndpoint(this);
        }
    }

    [RelayCommand]
    public void Pause()
    {
        if (_isShutdown)
        {
            return;
        }
        
        Active = false;
        _hostingService.ShutdownEndpointById(Id);
        SaveMockEndpoint();
    }
    
    [RelayCommand]
    public void Continue()
    {
        if (_isShutdown)
        {
            return;
        }
        if (Active)
        {
            return;
        }

        Active = true;
        _hostingService.AddEndpoint(this);
        SaveMockEndpoint();
    }

    [RelayCommand]
    private async Task EditMockEndpointAsync()
    {
        if (MockEndpointsViewModel.Current is not null)
        {
            await MockEndpointsViewModel.Current.EditMockEndpointAsync(this);
        }
    }

    public void SaveMockEndpoint()
    {
        if (MockEndpointsViewModel.Current is not null)
        {
            MockEndpointsViewModel.Current.SaveMockEndpoint(this);
        }
    }

    [RelayCommand]
    private void OpenInBrowser()
    {
        if (Route[0] is not '/')
        {
            Route = $"/{Route}";
        }
        if (Route[^1] is not '/')
        {
            Route = $"{Route}/";
        }

        SaveMockEndpoint();

        Process.Start(new ProcessStartInfo($"http://localhost:{Port}{Route}") { UseShellExecute = true });
    }

    [RelayCommand]
    private void DeleteMockEndpoint()
    {
        if (MockEndpointsViewModel.Current is not null)
        {
            MockEndpointsViewModel.Current.DeleteEndpoint(this);
        }
    }
}