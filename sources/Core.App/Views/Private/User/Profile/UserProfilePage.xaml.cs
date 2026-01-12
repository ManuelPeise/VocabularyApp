using Core.App.ViewModels;

namespace Core.App.Views.Private.User.Profile;

public partial class UserProfilePage : ContentPage
{
	public UserProfilePage(UserProfilePageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}