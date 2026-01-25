using CommunityToolkit.Mvvm.ComponentModel;

namespace Shared.Models.Authentication
{
    public partial class AuthenticationRequestModel: ObservableObject
    {
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private string _password = string.Empty;
        [ObservableProperty]
        private bool _rememberMe = false;
    }
}
