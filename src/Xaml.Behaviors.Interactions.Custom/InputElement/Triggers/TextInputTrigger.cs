using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class TextInputTrigger : RoutedEventTriggerBase<TextInputEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TextInputEventArgs> RoutedEvent 
        => InputElement.TextInputEvent;

    static TextInputTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TextInputTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<KeyDownTrigger, string?>(nameof(Text));

    /// <summary>
    /// 
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <inheritdoc />
    protected override void Handler(object? sender, TextInputEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        var isTextSet = IsSet(TextProperty);
        var text = Text;
        var haveText = isTextSet && e.Text == text;

        if (!isTextSet || haveText)
        {
            Execute(e);
        }
    }
}
