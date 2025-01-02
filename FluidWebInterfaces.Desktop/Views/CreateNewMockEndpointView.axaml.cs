using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Views;

public partial class CreateNewMockEndpointView : Window
{
    public CreateNewMockEndpointView()
    {
        InitializeComponent();
    }

    private void OnClickCreateButton(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CreateNewMockEndpointViewModel viewModel)
        {
            return;
        }
        
        var created = viewModel.TryCreateMockEndpoint();

        if (MockEndpointsViewModel.Current is not null)
        {
            MockEndpointsViewModel.Current.RefreshViewModel();
        }

        if (created)
        {
            Close();
        }
    }
}