// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Validation behavior for range based controls like <see cref="Slider"/> value.
/// </summary>
public class SliderValidationBehavior : PropertyValidationBehavior<RangeBase, double>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SliderValidationBehavior"/> class.
    /// </summary>
    public SliderValidationBehavior()
    {
        Property = RangeBase.ValueProperty;
    }
}
