#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that registers a global hotkey (window-wide) to execute actions.
/// </summary>
public partial class GlobalHotkeyBehavior : StyledElementTrigger<Control>
{
    private TopLevel? _topLevel;

    /// <summary>
    /// Gets or sets the key to listen for.
    /// </summary>
    [StyledProperty]
    public partial Key Key { get; set; }

    /// <summary>
    /// Gets or sets the key modifiers to listen for.
    /// </summary>
    [StyledProperty]
    public partial KeyModifiers KeyModifiers { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _topLevel = TopLevel.GetTopLevel(AssociatedObject);
        if (_topLevel != null)
        {
            _topLevel.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (_topLevel != null)
        {
            _topLevel.RemoveRoutedEventHandler(InputElement.KeyDownEvent, OnKeyDown);
            _topLevel = null;
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key && e.KeyModifiers == KeyModifiers)
        {
            Interaction.ExecuteActions(AssociatedObject, Actions, e);
            e.Handled = true;
        }
    }
}
