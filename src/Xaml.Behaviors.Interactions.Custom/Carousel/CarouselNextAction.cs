// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Advances the target <see cref="Carousel"/> to the next page.
/// </summary>
public class CarouselNextAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Carousel"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Carousel?> CarouselProperty =
        AvaloniaProperty.Register<CarouselNextAction, Carousel?>(nameof(Carousel));

    /// <summary>
    /// Gets or sets the carousel instance this action will operate on.
    /// This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public Carousel? Carousel
    {
        get => GetValue(CarouselProperty);
        set => SetValue(CarouselProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var carousel = Carousel ?? sender as Carousel;
        carousel?.Next();

        return null;
    }
}
