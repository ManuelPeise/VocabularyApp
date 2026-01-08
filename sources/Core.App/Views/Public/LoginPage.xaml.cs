using Core.App.ViewModels;

namespace Core.App.Views.Public;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}