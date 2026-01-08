namespace Core.App.Views.Shared;

public partial class LoadingOverlay : ContentView
{

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(LoadingOverlay), false);

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public LoadingOverlay()
    {
        InitializeComponent();
    }

    private static void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LoadingOverlay overlay)
        {
            overlay.IsVisible = (bool)newValue;
        }
    }
}