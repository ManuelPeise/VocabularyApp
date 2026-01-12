using Core.App.ViewModels;
using Core.App.Views.Private.User.Profile;
using Core.App.Views.Public;
namespace Core.App
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            Routing.RegisterRoute("StartPage", typeof(StartPage));
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("UserProfilePage", typeof(UserProfilePage));
        }
    }
}
