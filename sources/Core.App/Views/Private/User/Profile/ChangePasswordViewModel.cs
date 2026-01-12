using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.App.Services.Interfaces;
using Core.App.ViewModels;
using Logic.Administration.Interfaces;
using Shared.Models.UserAdministration;

namespace Core.App.Views.Private.User.Profile
{
    public partial class ChangePasswordViewModel: PrivateViewModelBase
    {
        private readonly IUserAdministration _userAdministration;

        [ObservableProperty]
        private ChangePasswordModel _model = new();
       
        public ChangePasswordViewModel(ICurrentUserService currentUserService,
            ISecureStorageHandler secureStorageHandler, IUserAdministration userAdministration): 
            base(currentUserService, secureStorageHandler)
        {
            _userAdministration = userAdministration;
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
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
            // Backend-Call, z.B.:
            // var result = await _userAdministration.ChangePassword(OldPassword, NewPassword);
            // if (!result) { ErrorMessage = "Altes Passwort ist falsch."; return; }
            // Popup schließen, Erfolgsmeldung etc.
        }
    }
}
