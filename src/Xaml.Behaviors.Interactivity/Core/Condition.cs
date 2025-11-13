// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia;
using Avalonia.Data;
using Avalonia.Metadata;
namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Represents a reusable comparison that can be used by triggers and behaviors.
/// </summary>
public class Condition : AvaloniaObject
{
    private IDisposable? _bindingSubscription;

    /// <summary>
    /// Identifies the <seealso cref="Binding"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IBinding?> BindingProperty =
        AvaloniaProperty.Register<Condition, IBinding?>(nameof(Binding));

    internal static readonly StyledProperty<object?> BindingValueProperty =
        AvaloniaProperty.Register<Condition, object?>(nameof(BindingValue));

    /// <summary>
    /// Identifies the <seealso cref="ComparisonCondition"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ComparisonConditionType> ComparisonConditionProperty =
        AvaloniaProperty.Register<Condition, ComparisonConditionType>(nameof(ComparisonCondition));

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<Condition, object?>(nameof(Value));

    /// <summary>
    /// Identifies the <seealso cref="Property"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaProperty?> PropertyProperty =
        AvaloniaProperty.Register<Condition, AvaloniaProperty?>(nameof(Property));

    /// <summary>
    /// Identifies the <seealso cref="SourceName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> SourceNameProperty =
        AvaloniaProperty.Register<Condition, string?>(nameof(SourceName));

    /// <summary>
    /// Gets or sets the bound object to compare. This is an avalonia property.
    /// </summary>
    [AssignBinding]
    public IBinding? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// Gets or sets the type of comparison that is performed. This is an avalonia property.
    /// </summary>
    public ComparisonConditionType ComparisonCondition
    {
        get => GetValue(ComparisonConditionProperty);
        set => SetValue(ComparisonConditionProperty, value);
    }

    /// <summary>
    /// Gets or sets the value that is used during comparison. This is an avalonia property.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the avalonia property that will be monitored for changes.
    /// </summary>
    public AvaloniaProperty? Property
    {
        get => GetValue(PropertyProperty);
        set => SetValue(PropertyProperty, value);
    }

    /// <summary>
    /// Gets or sets the name of the element that supplies <see cref="Property"/>. When null, the associated object is used.
    /// </summary>
    public string? SourceName
    {
        get => GetValue(SourceNameProperty);
        set => SetValue(SourceNameProperty, value);
    }

    internal object? BindingValue
    {
        get => GetValue(BindingValueProperty);
        set => SetValue(BindingValueProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == BindingProperty)
        {
            if (change.GetNewValue<IBinding?>() is not null && Property is not null)
            {
                throw new InvalidOperationException("Condition cannot use both Property and Binding.");
            }

            _bindingSubscription?.Dispose();
            _bindingSubscription = null;

            var newBinding = change.GetNewValue<IBinding?>();
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
