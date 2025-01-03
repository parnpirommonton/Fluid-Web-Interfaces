using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using FluidWebInterfaces.Desktop.ViewModels;

namespace FluidWebInterfaces.Desktop.Views;

public partial class CreateNewInstanceView : Window
{
    public CreateNewInstanceView()
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

    private void OnClickCreateButton(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not CreateNewInstanceViewModel viewModel)
        {
            return;
        }
        
        var created = viewModel.TryCreateInstance();

        if (ActiveInstancesViewModel.Current is not null)
        {
            ActiveInstancesViewModel.Current.RefreshViewModel();
        }

        if (created)
        {
            Close();
        }
    }
}