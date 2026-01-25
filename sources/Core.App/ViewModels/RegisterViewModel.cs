using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Administration.Interfaces;
using Shared.Interfaces;
using Shared.Models.UserAdministration;

namespace Core.App.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IUserAdministration _userAdministration;
        private readonly IHttpClient<UserRegistrationResult> _registrationClient;
        [ObservableProperty]
        private string _firstName = string.Empty;
        [ObservableProperty]
        private string _lastName = string.Empty;
        [ObservableProperty]
        private string _userName = string.Empty;
        [ObservableProperty]
        private DateTime _dateOfBirth;
        [ObservableProperty]
        private string _password = string.Empty;
        [ObservableProperty]
        private string _passwordReplication = string.Empty;

        [ObservableProperty]
        private DateTime _maxDate = DateTime.Now.Date;
        [ObservableProperty]
        private DateTime _minDate = new DateTime(1980, 01, 01);
        [ObservableProperty]
        private string? _errorMessage;

        
        public RegisterViewModel(IUserAdministration userAdministration, IHttpClient<UserRegistrationResult> registrationClient)
        {
            _userAdministration = userAdministration;
            _registrationClient = registrationClient;
            IsBusy = false;
        }

        [RelayCommand]
        private async Task NavigateToStart()
        {
            await Shell.Current.GoToAsync("StartPage");
        }

        [RelayCommand]
        private async Task CreateAccount()
        {
            ErrorMessage = null;

            try
            {
                if (Password.Length < 6 || Password != PasswordReplication)
                {
                    return;
                }

                IsBusy = true;

                if(await _registrationClient.IsApiAvailableAsync)
                {

                }

                var registration = await _userAdministration.CreateUserProfile(new UserRegistrationRequestModel
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = UserName,
                    DateOfBirth = DateOfBirth,
                    Password = Password,
                });

                if (registration.Result)
                {
                    await Shell.Current.GoToAsync("LoginPage", new Dictionary<string, object>
                    {
                        { "userName", UserName }
                    });
                }
                else
                {
                    ErrorMessage = Resx.Core.LabelRegistrationError;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
