// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <content>
/// Uno Platform specific members that mirror the Avalonia property system API used by derived types.
/// </content>
public abstract partial class Action
{
    private DependencyObject? _host;

    /// <summary>
    /// Gets the object hosting the trigger that owns this action, when known.
    /// </summary>
    /// <remarks>
    /// WinUI has no logical tree; the owning <see cref="ActionCollection"/> publishes the associated object
    /// of its trigger so actions can reach it (Avalonia uses the logical parent chain instead).
    /// </remarks>
    internal DependencyObject? Host => _host;

    internal void AttachToHost(DependencyObject host)
    {
        if (ReferenceEquals(_host, host))
        {
            return;
        }

        if (_host is not null)
        {
            DetachFromHost();
        }

        _host = host;

        if (this is IActionLogicalTreeLifecycle lifecycle)
        {
            lifecycle.AttachedToActionLogicalTree();
        }
    }

    internal void DetachFromHost()
    {
        if (_host is null)
        {
            return;
        }

        if (this is IActionLogicalTreeLifecycle lifecycle)
        {
            lifecycle.DetachedFromActionLogicalTree();
        }

        _host = null;
    }

    /// <summary>
    /// Called when the value of a dependency property declared with the generated property attributes changes.
    /// </summary>
    /// <remarks>
    /// Mirrors Avalonia's <c>OnPropertyChanged(AvaloniaPropertyChangedEventArgs)</c>; the generated
    /// dependency properties route their change notifications here.
    /// </remarks>
    /// <param name="change">The change details.</param>
    protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs change)
    {
    }

    /// <summary>
    /// Checks whether a local value is set for the specified property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns><c>true</c> when a local value is set; otherwise <c>false</c>.</returns>
    protected bool IsSet(DependencyProperty property) => AvaloniaObjectCompatExtensions.IsSet(this, property);

    /// <summary>
    /// Sets the value of a property. WinUI has no current-value layer, so a local value is set.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="value">The value.</param>
    protected void SetCurrentValue(DependencyProperty property, object? value) => SetValue(property, value);

    /// <summary>
    /// Updates the backing field of a field-backed property and publishes the value to the dependency property.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="property">The property.</param>
    /// <param name="field">The backing field.</param>
    /// <param name="value">The new value.</param>
    /// <returns><c>true</c> when the value changed.</returns>
    protected bool SetAndRaise<T>(DependencyProperty property, ref T field, T value)
        => AvaloniaObjectCompatExtensions.SetAndRaise(this, property, ref field, value);
}
