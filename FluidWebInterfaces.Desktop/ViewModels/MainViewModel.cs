using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluidWebInterfaces.Desktop.Services;

namespace FluidWebInterfaces.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IAsyncNavigationService<MainViewModel> _navigator;

    [ObservableProperty]
    private bool _isPageActiveInstances = true;

    [ObservableProperty]
    private bool _isPageMockEndpoints;
    
    [ObservableProperty]
    private bool _isPageDocumentation;

    public MainViewModel(IAsyncNavigationService<MainViewModel> navigator)
    {
        _navigator = navigator;
    }

    [RelayCommand]
    public async Task NavigateToActiveInstancesAsync()
    {
        IsPageActiveInstances = true;
        IsPageMockEndpoints = false;
        IsPageDocumentation = false;
        await _navigator.NavigateToViewOfModelAsync<ActiveInstancesViewModel>();
    }
    
    [RelayCommand]
    public async Task NavigateToMockEndpointsAsync()
    {
        IsPageMockEndpoints = true;
        IsPageActiveInstances = false;
        IsPageDocumentation = false;
        await _navigator.NavigateToViewOfModelAsync<MockEndpointsViewModel>();
    }
    
    [RelayCommand]
    public async Task NavigateToDocumentationAsync()
    {
        IsPageDocumentation = true;
        IsPageMockEndpoints = false;
        IsPageActiveInstances = false;
        await _navigator.NavigateToViewOfModelAsync<DocumentationViewModel>();
    }
}