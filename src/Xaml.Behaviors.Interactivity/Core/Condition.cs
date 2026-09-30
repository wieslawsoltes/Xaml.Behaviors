// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
#else
using Avalonia;
using Avalonia.Data;
using Avalonia.Metadata;
#endif
#if UNO
namespace Xaml.Interactivity;
#else
namespace Avalonia.Xaml.Interactivity;
#endif

/// <summary>
/// Represents a reusable comparison that can be used by triggers and behaviors.
/// </summary>
public partial class Condition : AvaloniaObject
{
#if UNO
    /// <summary>
    /// Gets or sets the value to compare. This is a dependency property.
    /// </summary>
    /// <remarks>
    /// WinUI applies a binding assigned in XAML to this property, so its value is the bound value that is compared
    /// (Avalonia assigns the binding itself and evaluates it into <see cref="BindingValue"/>).
    /// </remarks>
    [StyledProperty]
    public partial object? Binding { get; set; }

    /// <summary>
    /// Identifies the value compared by the condition; on Uno Platform the same property as <see cref="BindingProperty"/>.
    /// </summary>
    internal static DependencyProperty BindingValueProperty => BindingProperty;

    internal object? BindingValue => Binding;
#else
    private IDisposable? _bindingSubscription;

    /// <summary>
    /// Gets or sets the bound object to compare. This is an avalonia property.
    /// </summary>
    [StyledProperty(AssignBinding = true)]
    public partial BindingBase? Binding { get; set; }
#endif

    /// <summary>
    /// Gets or sets the type of comparison that is performed. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial ComparisonConditionType ComparisonCondition { get; set; }

    /// <summary>
    /// Gets or sets the value that is used during comparison. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial object? Value { get; set; }

    /// <summary>
    /// Gets or sets the avalonia property that will be monitored for changes.
    /// </summary>
    [StyledProperty]
    public partial AvaloniaProperty? Property { get; set; }

    /// <summary>
    /// Gets or sets the name of the element that supplies <see cref="Property"/>. When null, the associated object is used.
    /// </summary>
    [StyledProperty]
    public partial string? SourceName { get; set; }

#if UNO
    /// <summary>
    /// Called when the value of a dependency property of the condition changes.
    /// </summary>
    /// <param name="change">The change details.</param>
    protected virtual void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == BindingProperty && change.NewValue is not null && Property is not null)
        {
            throw new InvalidOperationException("Condition cannot use both Property and Binding.");
        }
#else
    [StyledProperty]
    internal partial object? BindingValue { get; set; }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BindingProperty)
        {
            if (change.GetNewValue<BindingBase?>() is not null && Property is not null)
            {
                throw new InvalidOperationException("Condition cannot use both Property and Binding.");
            }

            _bindingSubscription?.Dispose();
            _bindingSubscription = null;

            var newBinding = change.GetNewValue<BindingBase?>();
            if (newBinding is not null)
            {
                _bindingSubscription = this.Bind(BindingValueProperty, newBinding);
            }
            else
            {
                SetValue(BindingValueProperty, null);
            }
        }
#endif

        if (change.Property == PropertyProperty &&
            change.GetNewValue<AvaloniaProperty?>() is not null &&
            Binding is not null)
        {
            throw new InvalidOperationException("Condition cannot use both Property and Binding.");
        }
    }
}
