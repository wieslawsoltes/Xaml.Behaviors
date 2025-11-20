# CarouselNextAction

The `CarouselNextAction` advances the target `Carousel` to the next page.

### Properties

| Property | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Carousel` | `Carousel` | `null` | The target `Carousel` to control. If not set, the action attempts to use the `sender` as the target. |

### Usage Example

```xml
<Button Content="Next">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <CarouselNextAction Carousel="{Binding ElementName=MyCarousel}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
