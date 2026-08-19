// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks whether a value is within a specified range.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
[SuppressMessage("AvaloniaProperty", "AVP1002:AvaloniaProperty objects should not be owned by a generic type")]
public class RangeValidationRule<T> : AvaloniaObject, IValidationRule<T>
{
    /// <summary>
    /// Identifies the <see cref="Minimum"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<T?> MinimumProperty =
        AvaloniaProperty.Register<RangeValidationRule<T>, T?>(nameof(Minimum));

    /// <summary>
    /// Identifies the <see cref="Maximum"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<T?> MaximumProperty =
        AvaloniaProperty.Register<RangeValidationRule<T>, T?>(nameof(Maximum));

    /// <summary>
    /// Identifies the <see cref="ErrorMessage"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<RangeValidationRule<T>, string?>(
            nameof(ErrorMessage),
            defaultValue: "Value is out of range.");

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public T? Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public T? Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <inheritdoc />
    public string? ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    /// <inheritdoc />
    public bool Validate(T? value)
    {
        if (value is null || Minimum is null || Maximum is null)
        {
            return false;
        }
        
        if (Comparer<T>.Default.Compare(value, Minimum) < 0)
        {
            return false;
        }

        if (Comparer<T>.Default.Compare(value, Maximum) > 0)
        {
            return false;
        }

        return true;
    }
}
