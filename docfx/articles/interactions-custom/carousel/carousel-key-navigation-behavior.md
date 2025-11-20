# CarouselKeyNavigationBehavior

The `CarouselKeyNavigationBehavior` enables keyboard navigation for a `Carousel` using arrow keys. It allows users to switch between carousel items using Left/Right or Up/Down keys depending on the configured orientation.

### Properties

| Property | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Orientation` | `Orientation` | `Horizontal` | Specifies the orientation for navigation keys. `Horizontal` uses Left/Right arrows; `Vertical` uses Up/Down arrows. |

### Usage Example

```xml
<Carousel Name="MyCarousel">
    <Interaction.Behaviors>
        <CarouselKeyNavigationBehavior Orientation="Horizontal" />
    </Interaction.Behaviors>
    <TextBlock Text="Page 1" />
    <TextBlock Text="Page 2" />
    <TextBlock Text="Page 3" />
</Carousel>
```
