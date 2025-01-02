using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace FluidWebInterfaces.Desktop.Views;

public partial class ActiveInstancesErrorView : Window
{
    public ActiveInstancesErrorView()
    {
        InitializeComponent();
    }

    private void OnClickCloseButton(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}