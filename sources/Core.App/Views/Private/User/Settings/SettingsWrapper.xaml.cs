using System.Net.Http;

namespace Core.App.Views.Private.User.Settings;

public partial class SettingsWrapper : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(SettingsWrapper), string.Empty);

    public static readonly BindableProperty SettingContentProperty =
        BindableProperty.Create(nameof(SettingContent), typeof(View), typeof(SettingsWrapper), null);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public View SettingContent
    {
        get => (View)GetValue(SettingContentProperty);
        set => SetValue(SettingContentProperty, value);
    }


    public SettingsWrapper()
	{
		InitializeComponent();
	}
}