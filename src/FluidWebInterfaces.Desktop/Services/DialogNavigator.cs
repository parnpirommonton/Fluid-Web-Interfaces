using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using FluidWebInterfaces.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FluidWebInterfaces.Desktop.Services;

public class DialogNavigator : IAsyncNavigationService
{
    private readonly MainWindow _mainWindow;
    private readonly IServiceProvider _serviceProvider;

    public DialogNavigator(MainWindow mainWindow, IServiceProvider serviceProvider)
    {
        _mainWindow = mainWindow;
        _serviceProvider = serviceProvider;
    }

    public Task NavigateToViewOfModelAsync<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase
    {
        return NavigateToViewOfModelAsync(typeof(TViewModel), parameter);
    }
    public async Task NavigateToViewOfModelAsync(Type viewModelType, object? parameter = null)
    {
        var viewModel = (ViewModelBase?)_serviceProvider.GetRequiredService(viewModelType);

        if (viewModel is null)
        {
            throw new Exception($"Could not create {nameof(ViewModelBase)} from type {viewModelType}.");
        }

        var view = CreateViewInstanceFromModel(viewModelType);
        view.DataContext = viewModel;
        await viewModel.InitializeAsync(parameter);
        await view.ShowDialog(_mainWindow);
    }

    private Window CreateViewInstanceFromModel(Type viewModelType)
    {
        var viewName = viewModelType.Name[..^5];
        var viewType = Type.GetType($"FluidWebInterfaces.Desktop.Views.{viewName}");

        if (viewType is null)
        {
            throw new Exception($"View with full type name {viewType} does not exist.");
        }

        var view = (Window?)Activator.CreateInstance(viewType);

        if (view is null)
        {
            throw new Exception($"Unable to create view with type {viewType}.");
        }

        return view;
    }
}