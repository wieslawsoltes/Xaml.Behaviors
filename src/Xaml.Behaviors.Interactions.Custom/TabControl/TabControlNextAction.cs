using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Advances the target <see cref="TabControl"/> to the next tab.
/// </summary>
public class TabControlNextAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="TabControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TabControl?> TabControlProperty =
        AvaloniaProperty.Register<TabControlNextAction, TabControl?>(nameof(TabControl));

    /// <summary>
    /// Gets or sets the tab control instance this action will operate on. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public TabControl? TabControl
    {
        get => GetValue(TabControlProperty);
        set => SetValue(TabControlProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var tabControl = TabControl ?? sender as TabControl;
        if (tabControl is null)
        {
            return false;
        }

        if (tabControl.SelectedIndex < tabControl.ItemCount - 1)
        {
            tabControl.SelectedIndex += 1;
        }

        return null;
    }
}
