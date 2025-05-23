using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that toggles between a display element and an editor element
/// allowing inline editing of list items.
/// </summary>
public class InlineEditBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Identifies the display element.
    /// </summary>
    public static readonly StyledProperty<Control?> DisplayProperty =
        AvaloniaProperty.Register<InlineEditBehavior, Control?>(nameof(Display));

    /// <summary>
    /// Identifies the editor <see cref="TextBox"/> element.
    /// </summary>
    public static readonly StyledProperty<TextBox?> EditorProperty =
        AvaloniaProperty.Register<InlineEditBehavior, TextBox?>(nameof(Editor));

    /// <summary>
    /// Gets or sets the display element.
    /// </summary>
    [ResolveByName]
    public Control? Display
    {
        get => GetValue(DisplayProperty);
        set => SetValue(DisplayProperty, value);
    }

    /// <summary>
    /// Gets or sets the editor element.
    /// </summary>
    [ResolveByName]
    public TextBox? Editor
    {
        get => GetValue(EditorProperty);
        set => SetValue(EditorProperty, value);
    }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (Display is not null && Editor is not null)
        {
            Display.IsVisible = true;
            Editor.IsVisible = false;
        }

        var input = AssociatedObject as InputElement;
        if (input is null || Editor is null || Display is null)
        {
            return DisposableAction.Empty;
        }

        input.AddHandler(Gestures.DoubleTappedEvent, OnBeginEdit, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        input.AddHandler(InputElement.KeyDownEvent, OnBeginKeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        Editor.AddHandler(InputElement.KeyDownEvent, OnEditorKeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        Editor.AddHandler(InputElement.LostFocusEvent, OnEditorLostFocus, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        return DisposableAction.Create(() =>
        {
            input.RemoveHandler(Gestures.DoubleTappedEvent, OnBeginEdit);
            input.RemoveHandler(InputElement.KeyDownEvent, OnBeginKeyDown);
            Editor.RemoveHandler(InputElement.KeyDownEvent, OnEditorKeyDown);
            Editor.RemoveHandler(InputElement.LostFocusEvent, OnEditorLostFocus);
        });
    }

    private void OnBeginEdit(object? sender, RoutedEventArgs e)
    {
        BeginEdit();
    }

    private void OnBeginKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.F2)
        {
            BeginEdit();
            e.Handled = true;
        }
    }

    private void OnEditorKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Enter or Key.Escape)
        {
            EndEdit();
            e.Handled = true;
        }
    }

    private void OnEditorLostFocus(object? sender, RoutedEventArgs e)
    {
        EndEdit();
    }

    private void BeginEdit()
    {
        if (Display is null || Editor is null)
        {
            return;
        }

        if (!Editor.IsVisible)
        {
            Display.IsVisible = false;
            Editor.IsVisible = true;
            Editor.Focus();
            Editor.SelectAll();
        }
    }

    private void EndEdit()
    {
        if (Display is null || Editor is null)
        {
            return;
        }

        if (Editor.IsVisible)
        {
            Display.IsVisible = true;
            Editor.IsVisible = false;
        }
    }
}
