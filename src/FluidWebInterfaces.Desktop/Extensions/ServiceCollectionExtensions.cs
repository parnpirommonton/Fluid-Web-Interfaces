using System.IO;
using FluidWebInterfaces.Core.Logging;
using FluidWebInterfaces.Desktop.Services;
using FluidWebInterfaces.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FluidWebInterfaces.Desktop.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton(provider => provider);
        
        // Add logging to application.
        services.AddTransient<ILogger>(_ =>
        {
            var logLevel = (LogLevel)int.Parse(
                File.ReadAllText(PersistenceLocations.LogLevelFile));
            
            var logger = new Logger();
            logger.AddConsoleLogging();
            logger.FilterToLevel(logLevel);
            logger.AddFileLogging(PersistenceLocations.LogOutputFile);

            return logger;
        });
        
        // Add services to the application.
        services.AddSingleton<IInstanceService, InstanceService>();
        services.AddSingleton<INavigationService<MainViewModel>, MainViewModelNavigator>();
        services.AddSingleton<IAsyncNavigationService<MainViewModel>, MainViewModelNavigator>();
        services.AddSingleton<IAsyncNavigationService, DialogNavigator>();
        services.AddSingleton<IMockEndpointService, MockEndpointService>();
        services.AddSingleton<IContentTypeTemplateService, ContentTypeTemplateService>();
        services.AddSingleton<HostingService>();
        
        // Add view models to the application.
        // Navigation structure of application.
        services.AddSingleton<MainViewModel>()
                    .AddSingleton<ActiveInstancesViewModel>()
                    .AddSingleton<MockEndpointsViewModel>()
                    .AddSingleton<DocumentationViewModel>();
        // Application window dialogs.
        services.AddTransient<CreateNewInstanceViewModel>();
        services.AddTransient<EditActiveInstanceViewModel>();
        services.AddTransient<CreateNewMockEndpointViewModel>();
        services.AddTransient<EditMockEndpointViewModel>();
        services.AddTransient<ActiveInstancesErrorViewModel>();
        
        return services;
    }
}