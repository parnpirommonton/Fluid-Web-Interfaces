using System;
using System.Threading.Tasks;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Services;

public interface IAsyncNavigationService
{
    Task NavigateToViewOfModelAsync<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase;
    Task NavigateToViewOfModelAsync(Type viewModelType, object? parameter = null);
}

public interface IAsyncNavigationService<TViewModelRoot> : IAsyncNavigationService where TViewModelRoot : ViewModelBase
{
}