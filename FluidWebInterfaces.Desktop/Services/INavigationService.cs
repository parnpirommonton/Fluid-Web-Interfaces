using System;
using System.Threading.Tasks;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Services;

public interface INavigationService
{
    void NavigateToViewOfModel<TViewModel>(object? parameter = null) where TViewModel : ViewModelBase;
    void NavigateToViewOfModel(Type viewModelType, object? parameter = null);
}

public interface INavigationService<TViewModelRoot> : INavigationService where TViewModelRoot : ViewModelBase
{
}