// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Avalonia.Controls;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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
