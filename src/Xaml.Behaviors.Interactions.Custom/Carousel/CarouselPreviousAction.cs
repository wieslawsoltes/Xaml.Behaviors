using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Moves the target <see cref="Carousel"/> to the previous page.
/// </summary>
public class CarouselPreviousAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Carousel"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Carousel?> CarouselProperty =
        AvaloniaProperty.Register<CarouselPreviousAction, Carousel?>(nameof(Carousel));

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
        carousel?.Previous();

        return null;
    }
}
