using Core.App.ViewModels;

namespace Core.App.Views.Public;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}