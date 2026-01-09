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
    public NavigationBar()
	{
		InitializeComponent();
	}

    private void OnMenuButtonClicked(object sender, EventArgs e)
    {
        Shell.Current.FlyoutIsPresented = !Shell.Current.FlyoutIsPresented;
    }
}