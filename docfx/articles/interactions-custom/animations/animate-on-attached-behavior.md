# AnimateOnAttachedBehavior

Similar to `PlayAnimationBehavior`, but also supports `IAnimationBuilder` for dynamic animations.

### Properties
*   `Animation`: The animation to run.
*   `AnimationBuilder`: An implementation of `IAnimationBuilder` to create the animation.

### Example with AnimationBuilder

```xml
<Border Background="Green" Width="100" Height="100">
    <Interaction.Behaviors>
        <AnimateOnAttachedBehavior AnimationBuilder="{Binding MyCustomAnimationBuilder}" />
    </Interaction.Behaviors>
</Border>
```
