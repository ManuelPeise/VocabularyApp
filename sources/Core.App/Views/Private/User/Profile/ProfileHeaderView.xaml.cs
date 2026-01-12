using System.Windows.Input;

namespace Core.App.Views.Private.User.Profile;

public partial class ProfileHeaderView : ContentView
{
    public static readonly BindableProperty UserNameProperty =
           BindableProperty.Create(nameof(UserName), typeof(string), typeof(ProfileHeaderView), string.Empty);

    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    public static readonly BindableProperty ProfileImageProperty =
           BindableProperty.Create(nameof(ProfileImage), typeof(ImageSource), typeof(ProfileHeaderView), null);

    public ImageSource ProfileImage
    {
        get => (ImageSource)GetValue(ProfileImageProperty);
        set => SetValue(ProfileImageProperty, value);
    }

    public static readonly BindableProperty ChangeImageCommandProperty =
       BindableProperty.Create(
           nameof(ChangeImageCommand),
           typeof(ICommand),
           typeof(ProfileHeaderView));

    public ICommand ChangeImageCommand
    {
        get => (ICommand)GetValue(ChangeImageCommandProperty);
        set => SetValue(ChangeImageCommandProperty, value);
    }

    public ProfileHeaderView()
    {
        InitializeComponent();
    }

  
}