using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Services;

public class MainViewModelNavigator : INavigationService<MainViewModel>, IAsyncNavigationService<MainViewModel>
{
    private readonly IServiceProvider _provider;
    private readonly MainWindow _mainWindow;

    public MainViewModelNavigator(IServiceProvider provider, MainWindow mainWindow)
    {
        _provider = provider;
        _mainWindow = mainWindow;
    }

    public void NavigateToViewOfModel<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase
    {
        NavigateToViewOfModel(typeof(TViewModel));
    }

    public Task NavigateToViewOfModelAsync<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase
    {
        return NavigateToViewOfModelAsync(typeof(TViewModel));
    }

    public void NavigateToViewOfModel(Type viewModelType, object? parameter = null)
    {
        var viewModel = (ViewModelBase?)_provider.GetService(viewModelType);

        if (viewModel is null)
        {
            throw new Exception($"Could not create {nameof(ViewModelBase)} from type {viewModelType}.");
        }

        var view = CreateViewInstanceFromModel(viewModelType);
        view.DataContext = viewModel;

        _mainWindow.ViewControl.Content = view;
        
        viewModel.Initialize(parameter);
    }

    public async Task NavigateToViewOfModelAsync(Type viewModelType, object? parameter = null)
    {
        var viewModel = (ViewModelBase?)_provider.GetService(viewModelType);

        if (viewModel is null)
        {
            throw new Exception($"Could not create {nameof(ViewModelBase)} from type {viewModelType}.");
        }

        var view = CreateViewInstanceFromModel(viewModelType);
        view.DataContext = viewModel;
        
        _mainWindow.ViewControl.Content = view;
        
        await viewModel.InitializeAsync(parameter);
    }

    private Control CreateViewInstanceFromModel(Type viewModelType)
    {
        var viewName = viewModelType.Name[..^5];
        var viewType = Type.GetType($"FluidWebInterfaces.Desktop.Views.{viewName}");

        if (viewType is null)
        {
            throw new Exception($"View with full type name {viewType} does not exist.");
        }

        var view = (Control?)Activator.CreateInstance(viewType);

        if (view is null)
        {
            throw new Exception($"Unable to create view with type {viewType}.");
        }

        return view;
    }
}