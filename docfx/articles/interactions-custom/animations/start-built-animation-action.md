# StartBuiltAnimationAction

Starts an animation that is either provided directly or built using an `IAnimationBuilder`.

### Properties
*   `Animation`: The animation to run.
*   `AnimationBuilder`: The builder to create the animation.

### Example

```xml
<Button Content="Dynamic Animation">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <StartBuiltAnimationAction AnimationBuilder="{Binding RandomAnimationBuilder}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
