using Core.App.ViewModels;
using Services.Shared.Interfaces;

namespace Core.App.Views.Private.User.Settings
{
    public class SettingsPageViewModel : PrivateViewModelBase
    {
        public SettingsPageViewModel(ICurrentUserService currentUserService, ISecureStorageHandler secureStorageHandler)
            : base(currentUserService, secureStorageHandler)
        {

        }
    }
}
