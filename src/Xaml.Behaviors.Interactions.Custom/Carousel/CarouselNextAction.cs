// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Advances the target <see cref="Carousel"/> to the next page.
/// </summary>
public partial class CarouselNextAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the carousel instance this action will operate on.
    /// This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Carousel? Carousel { get; set; }

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
