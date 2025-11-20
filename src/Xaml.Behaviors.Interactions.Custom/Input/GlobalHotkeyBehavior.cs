using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that registers a global hotkey (window-wide) to execute actions.
/// </summary>
public class GlobalHotkeyBehavior : StyledElementTrigger<Control>
{
    private TopLevel? _topLevel;

    /// <summary>
    /// Identifies the <seealso cref="Key"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Key> KeyProperty =
        AvaloniaProperty.Register<GlobalHotkeyBehavior, Key>(nameof(Key));

    /// <summary>
    /// Identifies the <seealso cref="KeyModifiers"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<KeyModifiers> KeyModifiersProperty =
        AvaloniaProperty.Register<GlobalHotkeyBehavior, KeyModifiers>(nameof(KeyModifiers));

    /// <summary>
    /// Gets or sets the key to listen for.
    /// </summary>
    public Key Key
    {
        get => GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the key modifiers to listen for.
    /// </summary>
    public KeyModifiers KeyModifiers
    {
        get => GetValue(KeyModifiersProperty);
        set => SetValue(KeyModifiersProperty, value);
    }

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
            _topLevel.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
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
