using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FluidWebInterfaces.Desktop.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    public virtual void Initialize(object? parameter = null)
    {
        return;   
    }
    
    public virtual Task InitializeAsync(object? parameter = null)
    {
        Initialize(parameter);
        return Task.CompletedTask;
    }
}