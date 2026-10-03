// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#if UNO
using Microsoft.UI.Xaml;
#else
using Avalonia;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Validation rule that checks that a numeric value is greater than or equal to a minimum value.
/// </summary>
/// <typeparam name="T">Type of value to validate.</typeparam>
[SuppressMessage("AvaloniaProperty", "AVP1002:AvaloniaProperty objects should not be owned by a generic type")]
#if UNO
// Uno's dependency object generator does not support generic types deriving directly from DependencyObject.
public partial class MinValueValidationRule<T> : ValidationRuleBase, IValidationRule<T>
#else
public partial class MinValueValidationRule<T> : AvaloniaObject, IValidationRule<T>
#endif
{

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    [StyledProperty]
    public partial T? MinValue { get; set; }

    /// <inheritdoc />
    [StyledProperty(DefaultValue = "Value is below minimum.")]
    public partial string? ErrorMessage { get; set; }

    /// <inheritdoc />
    public bool Validate(T? value)
    {
        if (value is null || MinValue is null)
        {
            return false;
        }
        
        if (Comparer<T>.Default.Compare(value, MinValue) < 0)
        {
            return false;
        }

        return true;
    }
}
