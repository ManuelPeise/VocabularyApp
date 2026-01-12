namespace Core.App.Views.Shared;

public partial class LabeledDatePicker : ContentView
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

    public static readonly BindableProperty DateProperty =
     BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(LabeledEntry), DateTime.MinValue);

    public DateTime Date
    {
        get => (DateTime)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }


    public LabeledDatePicker()
	{
		InitializeComponent();
	}
}