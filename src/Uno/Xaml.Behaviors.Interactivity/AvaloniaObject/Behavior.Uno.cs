// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <content>
/// Uno Platform specific members that mirror the Avalonia property system API used by derived types.
/// </content>
public abstract partial class Behavior : IDependencyPropertyChangedHandler
{
    void IDependencyPropertyChangedHandler.OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == Trigger.ActionsProperty || e.Property == StyledElementTrigger.ActionsProperty)
        {
            (e.OldValue as ActionCollection)?.SetHost(null);
            (e.NewValue as ActionCollection)?.SetHost(AssociatedObject);
        }

        OnPropertyChanged(e);
    }

    /// <summary>
    /// Publishes the associated object to the actions of a trigger (WinUI has no logical tree).
    /// </summary>
    /// <param name="host">The associated object, or <c>null</c> when detaching.</param>
    private void UpdateActionsHost(DependencyObject? host)
    {
        if (this is ITrigger { Actions: { } actions })
        {
            actions.SetHost(host);
        }
    }

    /// <summary>
    /// Called when the value of a dependency property declared by this library changes.
    /// </summary>
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
