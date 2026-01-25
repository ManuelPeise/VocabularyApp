using CommunityToolkit.Mvvm.ComponentModel;

namespace Services.Shared.UiModels
{
    public partial class ChangePasswordModel : ObservableObject
    {
        [ObservableProperty]
        private int _userId;
        [ObservableProperty]
        private string _currentPassword = string.Empty;
        [ObservableProperty]
        private string _newPassword = string.Empty;
        [ObservableProperty]
        private string _passwordReplication = string.Empty;
        [ObservableProperty]
        private string _errorMessage = string.Empty;
    }
}
