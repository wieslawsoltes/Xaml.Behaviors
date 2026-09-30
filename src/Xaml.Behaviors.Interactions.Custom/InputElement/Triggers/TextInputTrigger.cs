// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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
        get => (string?)GetValue(TextProperty);
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
