using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Shared.Interfaces;
using Shared.Models.Authentication;

namespace Core.App.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IUserAdministrationService _userAdministration;
        private readonly IHttpClient _httpClient;
        [ObservableProperty]
        private UserRegistrationRequestModel _registrationRequestModel;

        [ObservableProperty]
        private DateTime _maxDate = DateTime.Now.Date;
        [ObservableProperty]
        private DateTime _minDate = new DateTime(1980, 01, 01);
        [ObservableProperty]
        private string? _errorMessage;


        public RegisterViewModel(
            IHttpClient httpClient,
            IUserAdministrationService userAdministration,
            ISecureStorageHandler secureStorageHandler,
            ILocalizationResourceManager localizationResourceManager) : base(secureStorageHandler, localizationResourceManager)
        {
            _userAdministration = userAdministration;
            _httpClient = httpClient;
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
                if (RegistrationRequestModel.Password.Length < 6 || RegistrationRequestModel.Password != RegistrationRequestModel.PasswordReplication)
                {
                    return;
                }

                IsBusy = true;

                if (await _httpClient.IsApiAvailableAsync)
                {

                }

                var registration = await _userAdministration.CreateUserProfile(RegistrationRequestModel);

                if (registration.Result)
                {
                    await Shell.Current.GoToAsync("LoginPage", new Dictionary<string, object>
                    {
                        { "emailAddress", RegistrationRequestModel.EmailAddress }
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
