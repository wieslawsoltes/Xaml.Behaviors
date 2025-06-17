// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation rule that checks whether a value is within a specified range.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
public class RangeValidationRule<T> : IValidationRule<T>
{
    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public T? Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public T? Maximum { get; set; }

    /// <inheritdoc />
    public string? ErrorMessage { get; set; } = "Value is out of range.";

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
