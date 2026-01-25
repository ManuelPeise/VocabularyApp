using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.App.ViewModels;
using Services.Shared.Interfaces;
using Services.Shared.UiModels;
using System.ComponentModel;

namespace Core.App.Views.Private.User.Profile
{
    public partial class ChangePasswordViewModel : PrivateViewModelBase
    {
        private readonly IUserAdministrationService _userAdministration;

        [ObservableProperty]
        private ChangePasswordModel _model;
        [ObservableProperty]
        private bool _isNewPasswordEnabled = false;
        [ObservableProperty]
        private bool _isPasswordConfirmationEnabled = false;
        [ObservableProperty]
        private bool _canChangePassword = false;

        public ChangePasswordViewModel(ICurrentUserService currentUserService,
            ISecureStorageHandler secureStorageHandler, IUserAdministrationService userAdministration) :
            base(currentUserService, secureStorageHandler)
        {
            _userAdministration = userAdministration;
            Model = InitializeModel();
            Model.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Model.CurrentPassword) || Model.CurrentPassword.Length >= 8)
            {
                IsNewPasswordEnabled = true;
            }
            else
            {
                IsNewPasswordEnabled = false;
            }

            if (!string.IsNullOrEmpty(Model.NewPassword) || Model.NewPassword.Length >= 8)
            {
                IsPasswordConfirmationEnabled = true;
            }
            else
            {
                IsPasswordConfirmationEnabled = false;
            }

            if (!string.IsNullOrEmpty(Model.CurrentPassword) && Model.CurrentPassword.Length >= 8 &&
                !string.IsNullOrEmpty(Model.NewPassword) && Model.NewPassword.Length >= 8 &&
                Model.NewPassword == Model.PasswordReplication)
            {
                CanChangePassword = true;

                return;
            }

            CanChangePassword = false;
        }


        [RelayCommand]
        private async Task ChangePassword(Popup popup)
        {
            Model.ErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(Model.CurrentPassword) || string.IsNullOrWhiteSpace(Model.NewPassword) || string.IsNullOrWhiteSpace(Model.PasswordReplication))
            {
                Model.ErrorMessage = Resx.Core.LabelAllFieldsMustBeFilled;

                return;
            }

            if (Model.NewPassword != Model.NewPassword)
            {
                Model.ErrorMessage = Resx.Core.LabelPasswordsDoesNotMatch;

                return;
            }

            var result = await _userAdministration.ChangePassword(Model);

            if (!result.Success)
            {
                Model.ErrorMessage = result.ErrorMessage;

                return;
            }

            Model = InitializeModel();

            await popup.CloseAsync();
        }

        [RelayCommand]
        private async Task ClosePopup(Popup popup)
        {
            await popup.CloseAsync();
        }

        private ChangePasswordModel InitializeModel()
        {
            IsNewPasswordEnabled = false;
            IsPasswordConfirmationEnabled = false;
            CanChangePassword = false;
            return new ChangePasswordModel
            {
                UserId = CurrentUserService.UserData.UserId,
                CurrentPassword = string.Empty,
                NewPassword = string.Empty,
                PasswordReplication = string.Empty,
                ErrorMessage = string.Empty
            };
        }
    }
}
