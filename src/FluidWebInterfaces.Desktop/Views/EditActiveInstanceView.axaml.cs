using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Views;

public partial class EditActiveInstanceView : Window
{
    public EditActiveInstanceView()
    {
        InitializeComponent();
    }

    private async void OnClickSourceFolderButton(object? sender, RoutedEventArgs e)
    {
        var selectedFolder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false,
            Title = "Select source folder"
        });
        SourceFolderTextBox.Text = selectedFolder.FirstOrDefault()?.Path.AbsolutePath;
    }

    private async void OnClickOutputFolderButton(object? sender, RoutedEventArgs e)
    {
        var selectedFolder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false,
            Title = "Select output folder"
        });
        OutputFolderTextBox.Text = selectedFolder.FirstOrDefault()?.Path.AbsolutePath;
    }

    private void OnClickSaveChangesButton(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not EditActiveInstanceViewModel viewModel)
        {
            return;
        }
        
        var updated = viewModel.TryUpdateInstance();

        if (ActiveInstancesViewModel.Current is not null)
        {
            ActiveInstancesViewModel.Current.RefreshViewModel();
        }

        if (updated)
        {
            Close();
        }
    }
}