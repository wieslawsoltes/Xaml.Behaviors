// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls.Primitives;

namespace Avalonia.Xaml.Interactions.Custom;

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
