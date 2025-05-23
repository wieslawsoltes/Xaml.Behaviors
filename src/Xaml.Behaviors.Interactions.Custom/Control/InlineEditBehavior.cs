using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that toggles visibility of display and edit controls to enable inline editing.
/// </summary>
public class InlineEditBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="EditControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> EditControlProperty =
        AvaloniaProperty.Register<InlineEditBehavior, Control?>(nameof(EditControl));

    /// <summary>
    /// Identifies the <see cref="DisplayControl"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Control?> DisplayControlProperty =
        AvaloniaProperty.Register<InlineEditBehavior, Control?>(nameof(DisplayControl));

    /// <summary>
    /// Identifies the <see cref="EditKey"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Key> EditKeyProperty =
        AvaloniaProperty.Register<InlineEditBehavior, Key>(nameof(EditKey), Key.F2);

    /// <summary>
    /// Identifies the <see cref="AcceptKey"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Key> AcceptKeyProperty =
        AvaloniaProperty.Register<InlineEditBehavior, Key>(nameof(AcceptKey), Key.Enter);

    /// <summary>
    /// Identifies the <see cref="CancelKey"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Key> CancelKeyProperty =
        AvaloniaProperty.Register<InlineEditBehavior, Key>(nameof(CancelKey), Key.Escape);

    /// <summary>
    /// Editing control to show when editing begins.
    /// </summary>
    [ResolveByName]
    public Control? EditControl
    {
        get => GetValue(EditControlProperty);
        set => SetValue(EditControlProperty, value);
    }

    /// <summary>
    /// Display control to show when not editing.
    /// </summary>
    [ResolveByName]
    public Control? DisplayControl
    {
        get => GetValue(DisplayControlProperty);
        set => SetValue(DisplayControlProperty, value);
    }

    /// <summary>
    /// Key used to start editing.
    /// </summary>
    public Key EditKey
    {
        get => GetValue(EditKeyProperty);
        set => SetValue(EditKeyProperty, value);
    }

    /// <summary>
    /// Key used to accept editing.
    /// </summary>
    public Key AcceptKey
    {
        get => GetValue(AcceptKeyProperty);
        set => SetValue(AcceptKeyProperty, value);
    }

    /// <summary>
    /// Key used to cancel editing.
    /// </summary>
    public Key CancelKey
    {
        get => GetValue(CancelKeyProperty);
        set => SetValue(CancelKeyProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (DisplayControl is not null)
        {
            DisplayControl.AddHandler(InputElement.DoubleTappedEvent, OnDisplayActivate, RoutingStrategies.Tunnel);
            DisplayControl.AddHandler(InputElement.KeyDownEvent, OnDisplayKeyDown, RoutingStrategies.Tunnel);
        }

        if (EditControl is not null)
        {
            EditControl.AddHandler(InputElement.KeyDownEvent, OnEditKeyDown, RoutingStrategies.Tunnel);
            EditControl.AddHandler(InputElement.LostFocusEvent, OnEditLostFocus, RoutingStrategies.Bubble);
            EditControl.IsVisible = false;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (DisplayControl is not null)
        {
            DisplayControl.RemoveHandler(InputElement.DoubleTappedEvent, OnDisplayActivate);
            DisplayControl.RemoveHandler(InputElement.KeyDownEvent, OnDisplayKeyDown);
        }

        if (EditControl is not null)
        {
            EditControl.RemoveHandler(InputElement.KeyDownEvent, OnEditKeyDown);
            EditControl.RemoveHandler(InputElement.LostFocusEvent, OnEditLostFocus);
        }
    }

    private void OnDisplayActivate(object? sender, RoutedEventArgs e) => BeginEdit();

    private void OnDisplayKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == EditKey)
        {
            BeginEdit();
            e.Handled = true;
        }
    }

    private void OnEditKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == AcceptKey || e.Key == CancelKey)
        {
            EndEdit();
        }
    }

    private void OnEditLostFocus(object? sender, RoutedEventArgs e) => EndEdit();

    private void BeginEdit()
    {
        if (EditControl is null || DisplayControl is null)
            return;

        DisplayControl.IsVisible = false;
        EditControl.IsVisible = true;
        EditControl.Focus();
        if (EditControl is TextBox tb)
        {
            tb.SelectAll();
        }
    }

    private void EndEdit()
    {
        if (EditControl is null || DisplayControl is null)
            return;

        EditControl.IsVisible = false;
        DisplayControl.IsVisible = true;
    }
}
