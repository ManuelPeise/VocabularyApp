
using System.Windows.Input;

namespace Core.App.Views.Shared;

public partial class FlyoutMenuHeader : ContentView
{
    public static readonly BindableProperty UserNameProperty =
      BindableProperty.Create(nameof(UserName), typeof(string), typeof(FlyoutMenuHeader), string.Empty);
   
    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    public static readonly BindableProperty LogoutCommandProperty =
       BindableProperty.Create(nameof(LogoutCommand), typeof(ICommand), typeof(FlyoutMenuHeader));

    public ICommand LogoutCommand
    {
        get => (ICommand)GetValue(LogoutCommandProperty);
        set => SetValue(LogoutCommandProperty, value);
    }

    public FlyoutMenuHeader()
	{
		InitializeComponent();
	}
}