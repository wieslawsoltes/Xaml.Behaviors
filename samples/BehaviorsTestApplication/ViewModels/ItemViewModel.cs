using System.Collections.ObjectModel;

namespace BehaviorsTestApplication.ViewModels;

public partial class ItemViewModel : ViewModelBase
{
    public ItemViewModel(string value, string color = "Black")
    {
        _value = value;
        _color = color;
    }

#if UNO
    // WinUI XAML has no x:Arguments: the Uno views create items with this constructor and property setters.
    public ItemViewModel() : this(string.Empty)
    {
    }
#endif

    [Reactive]
    public partial string? Value { get; set; }

    [Reactive]
    public partial ObservableCollection<ItemViewModel>? Items { get; set; }

    [Reactive]
    public partial string? Color { get; set; }

    public override string ToString() => _value ?? string.Empty;
}
