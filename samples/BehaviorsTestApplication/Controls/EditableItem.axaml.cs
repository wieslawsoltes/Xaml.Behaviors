#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
#endif

namespace BehaviorsTestApplication.Controls;

public partial class EditableItem : UserControl
{
#if UNO
    // WinUI has no default binding modes: bind Text with Mode=TwoWay.
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(EditableItem), new PropertyMetadata(null));
#else
    public static readonly StyledProperty<string?> TextProperty =
        TextBlock.TextProperty.AddOwner<EditableItem>(new(
            defaultBindingMode: BindingMode.TwoWay));
#endif

    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public EditableItem()
    {
        InitializeComponent();
    }

#if !UNO
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
#endif
}
