using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluidWebInterfaces.Desktop.Extensions;
using FluidWebInterfaces.Desktop.Services;
using FluidWebInterfaces.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FluidWebInterfaces.Desktop;

public partial class App : Application
{
    public IServiceProvider ServiceProvider = null!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        IServiceCollection collection = new ServiceCollection();
        
        // Add application services.
        collection.AddApplication();

        var serviceProvider = collection.BuildServiceProvider();
        ServiceProvider = serviceProvider;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            desktop.MainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();

            desktop.Exit += OnApplicationExit;
        }

        base.OnFrameworkInitializationCompleted();

        // Force the view models to load.
        serviceProvider.GetRequiredService<ActiveInstancesViewModel>().Initialize();
        serviceProvider.GetRequiredService<MockEndpointsViewModel>().Initialize();
        
        // Finally, navigate to the active instances view.
        var navigator = serviceProvider.GetRequiredService<INavigationService<MainViewModel>>();
        navigator.NavigateToViewOfModel<ActiveInstancesViewModel>();

        // Very crude fix, I will find out what's wrong with the HttpListener implementation later.
        foreach (var endpoint in MockEndpointsViewModel.Current!.MockEndpoints)
        {
            if (endpoint.Active)
            {
                endpoint.Pause();
                endpoint.Continue();
            }
        }
    }

    public void OnApplicationExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        var hostingService = ServiceProvider.GetRequiredService<HostingService>();
        hostingService.ShutdownAllInstances();
    }
}