// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks that a numeric value is less than or equal to a maximum value.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
[SuppressMessage("AvaloniaProperty", "AVP1002:AvaloniaProperty objects should not be owned by a generic type")]
public class MaxValueValidationRule<T> : AvaloniaObject, IValidationRule<T>
{
    /// <summary>
    /// Identifies the <see cref="MaxValue"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<T?> MaxValueProperty =
        AvaloniaProperty.Register<MaxValueValidationRule<T>, T?>(nameof(MaxValue));

    /// <summary>
    /// Identifies the <see cref="ErrorMessage"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<MaxValueValidationRule<T>, string?>(
            nameof(ErrorMessage),
            defaultValue: "Value is above maximum.");

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public T? MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
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
        if (value is null || MaxValue is null)
        {
            return false;
        }
        
        if (Comparer<T>.Default.Compare(value, MaxValue) > 0)
        {
            return false;
        }

        return true;
    }
}
