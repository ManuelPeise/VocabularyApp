using Services.Shared.UiModels;
using System.Collections.ObjectModel;

namespace Core.App.Views.Shared;

public partial class Dropdown : ContentView
{
    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(ObservableCollection<DropdownItem>), typeof(Dropdown), null);

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(DropdownItem), typeof(Dropdown), null, 
            BindingMode.TwoWay);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(Dropdown), null);

    public ObservableCollection<DropdownItem> Items
    {
        get => (ObservableCollection<DropdownItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public DropdownItem SelectedItem
    {
        get => (DropdownItem)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public Dropdown()
    {
        InitializeComponent();
    }
}