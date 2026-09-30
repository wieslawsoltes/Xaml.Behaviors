// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
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
/// Behavior that toggles visibility of display and edit controls to enable inline editing.
/// </summary>
public partial class InlineEditBehavior : StyledElementBehavior<Control>
{

    /// <summary>
    /// Editing control to show when editing begins.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? EditControl { get; set; }

    /// <summary>
    /// Display control to show when not editing.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? DisplayControl { get; set; }

    /// <summary>
    /// Key used to start editing.
    /// </summary>
    [StyledProperty(DefaultValue = Key.F2)]
    public partial Key EditKey { get; set; }

    /// <summary>
    /// Key used to accept editing.
    /// </summary>
    [StyledProperty(DefaultValue = Key.Enter)]
    public partial Key AcceptKey { get; set; }

    /// <summary>
    /// Key used to cancel editing.
    /// </summary>
    [StyledProperty(DefaultValue = Key.Escape)]
    public partial Key CancelKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether double tapping the associated object starts editing.
    /// </summary>
    [StyledProperty(DefaultValue = false)]
    public partial bool EditOnAssociatedObjectDoubleTapped { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (DisplayControl is not null)
        {
            DisplayControl.AddHandler(InputElement.DoubleTappedEvent, OnDisplayActivate, RoutingStrategies.Bubble);
            DisplayControl.AddHandler(InputElement.KeyDownEvent, OnDisplayKeyDown, RoutingStrategies.Tunnel);
        }

        if (EditOnAssociatedObjectDoubleTapped && AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.DoubleTappedEvent, OnAssociatedObjectActivate, RoutingStrategies.Bubble);
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
            DisplayControl.RemoveRoutedEventHandler(InputElement.DoubleTappedEvent, OnDisplayActivate);
            DisplayControl.RemoveRoutedEventHandler(InputElement.KeyDownEvent, OnDisplayKeyDown);
        }

        if (EditOnAssociatedObjectDoubleTapped && AssociatedObject is not null)
        {
            AssociatedObject.RemoveRoutedEventHandler(InputElement.DoubleTappedEvent, OnAssociatedObjectActivate);
        }

        if (EditControl is not null)
        {
            EditControl.RemoveRoutedEventHandler(InputElement.KeyDownEvent, OnEditKeyDown);
            EditControl.RemoveRoutedEventHandler(InputElement.LostFocusEvent, OnEditLostFocus);
        }
    }

    private void OnAssociatedObjectActivate(object? sender, RoutedEventArgs e) => BeginEdit();

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
        if (EditControl is null || DisplayControl is null || EditControl.IsVisible)
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
