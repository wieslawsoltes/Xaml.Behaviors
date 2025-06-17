// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Validation behavior for <see cref="NumericUpDown"/> value.
/// </summary>
public class NumericUpDownValidationBehavior : PropertyValidationBehavior<NumericUpDown, decimal?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericUpDownValidationBehavior"/> class.
    /// </summary>
    public NumericUpDownValidationBehavior()
    {
        Property = NumericUpDown.ValueProperty;
    }
}
