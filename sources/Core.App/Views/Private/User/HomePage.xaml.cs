using Core.App.ViewModels;

namespace Core.App.Views.Private.User;

public partial class HomePage : ContentPage
{
	public HomePage(HomePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}