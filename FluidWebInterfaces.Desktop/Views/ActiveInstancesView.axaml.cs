using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Views;

public partial class ActiveInstancesView : UserControl
{
    public ActiveInstancesView()
    {
        InitializeComponent();
    }

    private void OnClickInstanceMenuButton(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button menuButton)
        {
            return;
        }

        menuButton.ContextFlyout?.ShowAt(menuButton);
    }

    private void OnFilterTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DataContext is not ActiveInstancesViewModel viewModel)
        {
            return;
        }
        
        viewModel.RefreshFilteredActiveInstances();
    }
}