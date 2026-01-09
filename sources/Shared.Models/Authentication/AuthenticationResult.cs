using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Enums;

namespace Shared.Models.Authentication
{
    public partial class AuthenticationResult: ObservableObject
    {
        [ObservableProperty]
        private int _userId;
        [ObservableProperty]
        private string _userName;
        [ObservableProperty]
        private UserRoleEnum _userRole;
        [ObservableProperty]
        private bool _isAuthenticated;
    }
}
