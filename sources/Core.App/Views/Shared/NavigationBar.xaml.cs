using System.Windows.Input;

namespace Core.App.Views.Shared;

public partial class NavigationBar : ContentView
{
    public static readonly BindableProperty TitleProperty =
             BindableProperty.Create(nameof(Title), typeof(string), typeof(NavigationBar), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty LogoutCommandProperty =
        BindableProperty.Create(nameof(LogoutCommand), typeof(ICommand), typeof(NavigationBar));

    public ICommand LogoutCommand
    {
        get => (ICommand)GetValue(LogoutCommandProperty);
        set => SetValue(LogoutCommandProperty, value);
    }

    public static readonly BindableProperty NavigateToProfileCommandProperty =
       BindableProperty.Create(nameof(NavigateToProfileCommand), typeof(ICommand), typeof(NavigationBar));

    public ICommand NavigateToProfileCommand
    {
        get => (ICommand)GetValue(NavigateToProfileCommandProperty);
        set => SetValue(NavigateToProfileCommandProperty, value);
    }

    public static readonly BindableProperty ShowProfileIconProperty =
            BindableProperty.Create(nameof(ShowProfileIcon), typeof(bool), typeof(NavigationBar), true);

    public bool ShowProfileIcon
    {
        get => (bool)GetValue(ShowProfileIconProperty);
        set => SetValue(ShowProfileIconProperty, value);
    }

    public NavigationBar()
	{
		InitializeComponent();
	}

    private void OnMenuButtonClicked(object sender, EventArgs e)
    {
        Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
    }
}