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
    private IDisposable? _bindingSubscription;

    /// <summary>
    /// Gets or sets the bound object to compare. This is an avalonia property.
    /// </summary>
    [StyledProperty(AssignBinding = true)]
    public partial BindingBase? Binding { get; set; }

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

        if (change.Property == PropertyProperty &&
            change.GetNewValue<AvaloniaProperty?>() is not null &&
            Binding is not null)
        {
            throw new InvalidOperationException("Condition cannot use both Property and Binding.");
        }
    }
}
