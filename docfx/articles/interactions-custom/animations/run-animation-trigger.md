# RunAnimationTrigger

Similar to `AnimationCompletedTrigger`, but also supports `IAnimationBuilder`.

### Properties
*   `Animation`: The animation to run.
*   `AnimationBuilder`: An implementation of `IAnimationBuilder`.

### Example

```xml
<Border Background="Orange" Width="100" Height="100">
    <Interaction.Behaviors>
        <RunAnimationTrigger AnimationBuilder="{Binding MyComplexBuilder}">
            <CallMethodAction TargetObject="{Binding}" MethodName="OnAnimationFinished" />
        </RunAnimationTrigger>
    </Interaction.Behaviors>
</Border>
```
