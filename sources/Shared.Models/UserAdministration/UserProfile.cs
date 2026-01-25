using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Enums;

namespace Shared.Models.UserAdministration
{
    public partial class UserProfile: ObservableObject
    {
        [ObservableProperty] private int _userId;
        [ObservableProperty] private string _firstName;
        [ObservableProperty] private string _lastName;
        [ObservableProperty] private string _userName;
        [ObservableProperty] private string _Email;
        [ObservableProperty] private byte[] _profileImage;
        [ObservableProperty] private DateTime _dateOfBirth;
        [ObservableProperty] private UserRoleEnum _userRole;
        [ObservableProperty] private string _lastUpdateBy;
        [ObservableProperty] private string _lastUpdateAt;
    }
}
