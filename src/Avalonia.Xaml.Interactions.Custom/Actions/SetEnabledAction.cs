using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets the <see cref="Control.IsEnabled"/> property of a control when executed.
/// </summary>
public class SetEnabledAction : Avalonia.Xaml.Interactivity.StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TargetControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<SetEnabledAction, Control?>(nameof(TargetControl));

    /// <summary>
    /// Identifies the <seealso cref="IsEnabledValue"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> IsEnabledValueProperty =
        AvaloniaProperty.Register<SetEnabledAction, bool>(nameof(IsEnabledValue), true);

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to assign to <see cref="Control.IsEnabled"/>.
    /// This is an avalonia property.
    /// </summary>
    public bool IsEnabledValue
    {
        get => GetValue(IsEnabledValueProperty);
        set => SetValue(IsEnabledValueProperty, value);
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if the property is successfully updated; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var control = TargetControl ?? sender as Control;
        if (control is null)
        {
            return false;
        }

        control.IsEnabled = IsEnabledValue;
        return true;
    }
}
