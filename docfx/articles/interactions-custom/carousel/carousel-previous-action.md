# CarouselPreviousAction

The `CarouselPreviousAction` moves the target `Carousel` to the previous page.

### Properties

| Property | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Carousel` | `Carousel` | `null` | The target `Carousel` to control. If not set, the action attempts to use the `sender` as the target. |

### Usage Example

```xml
<Button Content="Previous">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <CarouselPreviousAction Carousel="{Binding ElementName=MyCarousel}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
