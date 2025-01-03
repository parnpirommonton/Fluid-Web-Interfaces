using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluidWebInterfaces.Core.Logging;
using FluidWebInterfaces.Desktop.Extensions;
using FluidWebInterfaces.Desktop.Services;
using FluidWebInterfaces.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FluidWebInterfaces.Desktop;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider = null!;
    public static ILogger Logger = null!;
    
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
        var logger = serviceProvider.GetRequiredService<ILogger>();
        Logger = logger;
        
        logger.Log(LogLevel.Debug, "ServiceProvider has been built");
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
            desktop.MainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();

            desktop.Exit += OnApplicationExit;
        }
        
        logger.Log(LogLevel.Debug, "Initializing view models");

        // Force the view models to load.
        serviceProvider.GetRequiredService<ActiveInstancesViewModel>().Initialize();
        serviceProvider.GetRequiredService<MockEndpointsViewModel>().Initialize();
        
        logger.Log(LogLevel.Debug, "Navigating to view model {viewmodel}", nameof(ActiveInstancesViewModel));
        
        // Finally, navigate to the active instances view.
        var navigator = serviceProvider.GetRequiredService<INavigationService<MainViewModel>>();
        navigator.NavigateToViewOfModel<ActiveInstancesViewModel>();
        
        logger.Log(LogLevel.Debug, "Activating Mock Endpoints");

        // Very crude fix, I will find out what's wrong with the HttpListener implementation later.
        foreach (var endpoint in MockEndpointsViewModel.Current!.MockEndpoints)
        {
            if (endpoint.Active)
            {
                endpoint.Pause();
                endpoint.Continue();
                logger.Log(LogLevel.Trace, "Mock Endpoint {endpoint} activated", endpoint.Id);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void OnApplicationExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        var hostingService = ServiceProvider.GetRequiredService<HostingService>();
        hostingService.ShutdownAllInstances();
        Logger.Log(LogLevel.Debug, "Shutting down all mock endpoints");
    }
}