using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Views;

public partial class EditMockEndpointView : Window
{
    public EditMockEndpointView()
    {
        InitializeComponent();
    }
    
    private void OnClickSaveChangesButton(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not EditMockEndpointViewModel viewModel)
        {
            return;
        }
        
        var updated = viewModel.TryUpdateMockEndpoint();

        if (MockEndpointsViewModel.Current is not null)
        {
            MockEndpointsViewModel.Current.RefreshViewModel();
        }

        if (updated)
        {
            Close();
        }
    }
}