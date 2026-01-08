
using CommunityToolkit.Mvvm.ComponentModel;

namespace Core.App.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;
    }
}
