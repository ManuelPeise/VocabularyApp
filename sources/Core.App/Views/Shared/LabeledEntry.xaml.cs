namespace Core.App.Views.Shared;

public partial class LabeledEntry : ContentView
{
    public static readonly BindableProperty LabelProperty =
       BindableProperty.Create(nameof(Label), typeof(string), typeof(LabeledEntry), string.Empty);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly BindableProperty IconProperty =
      BindableProperty.Create(nameof(Icon), typeof(string), typeof(LabeledEntry), string.Empty);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty =
      BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LabeledEntry), string.Empty);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty TextProperty =
       BindableProperty.Create(nameof(Text), typeof(string), typeof(LabeledEntry), string.Empty, BindingMode.TwoWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty IsPasswordProperty =
       BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(LabeledEntry), false);

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public LabeledEntry()
	{
		InitializeComponent();
	}
}