using Logic.Shared.Interfaces;
using Services.Shared.Interfaces;

namespace Core.App.ViewModels
{
    public partial class AppShellViewModel : PrivateViewModelBase
    {
        public AppShellViewModel(ICurrentUserService currentUserService, ISecureStorageHandler secureStorageHandler) : 
            base(currentUserService, secureStorageHandler) 
        {
            Task.Run(async () => await InitializeAsync());
        }

        private async Task InitializeAsync()
        {
            //var userDataJson = await SecureStorageHandler.GetAsync(StorageKeys.UserData);
            //AuthenticationResult? authenticationResult = null;
            
            //if(!string.IsNullOrEmpty(userDataJson))
            //{
            //    authenticationResult = JsonSerializer.Deserialize<AuthenticationResult>(userDataJson);
                
            //}

            //if(authenticationResult != null)
            //{
            //    CurrentUserService.SetCurrentUser(authenticationResult);
            //}
        }
    }
}
