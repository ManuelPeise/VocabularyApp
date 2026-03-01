using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Logic.Shared.Interfaces;
using System.ComponentModel;

namespace Core.App.ViewModels
{
    public partial class AppShellViewModel : PrivateViewModelBase
    {
     

        public AppShellViewModel(
            ICurrentUserService currentUserService,
            ISecureStorageHandler secureStorageHandler,
            ILocalizationResourceManager localizationResourceManager) :
            base(currentUserService, secureStorageHandler, localizationResourceManager)
        {
            
        }
    }
}
