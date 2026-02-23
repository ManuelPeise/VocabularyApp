using CommunityToolkit.Mvvm.ComponentModel;

namespace Shared.Models.Authentication
{
    public partial class UserRegistrationRequestModel : ObservableObject
    {
        [ObservableProperty] private string _firstName;
        [ObservableProperty] private string _lastName;
        [ObservableProperty] private string _emailAddress;
        [ObservableProperty] private DateTime _dateOfBirth;
        [ObservableProperty] private string _password;
        [ObservableProperty] private string _passwordReplication;
    }
}
