using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom.Automation;

/// <summary>
/// Raises <see cref="Button.ClickEvent"/> on the target button when executed.
/// </summary>
public class InvokeClickAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="TargetButton"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Button?> TargetButtonProperty =
        AvaloniaProperty.Register<InvokeClickAction, Button?>(nameof(TargetButton));

    /// <summary>
    /// Gets or sets the target button. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Button? TargetButton
    {
        get => GetValue(TargetButtonProperty);
        set => SetValue(TargetButtonProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var button = TargetButton ?? sender as Button;
        if (button is null)
        {
            return false;
        }

        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        return true;
    }
}
