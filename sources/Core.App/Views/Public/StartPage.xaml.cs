using Core.App.ViewModels;

namespace Core.App.Views.Public;

public partial class StartPage : ContentPage
{
	public StartPage(StartPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}