# BeginAnimationAction

Starts an animation on a specified `TargetControl`. If `TargetControl` is not set, it targets the associated object.

### Properties
*   `Animation`: The animation to run.
*   `TargetControl`: The control to animate.

### Example

```xml
<Button Content="Shake Box">
    <Button.Resources>
        <Animation x:Key="Shake" Duration="0:0:0.5">
            <!-- KeyFrames for shaking -->
        </Animation>
    </Button.Resources>
    
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <BeginAnimationAction Animation="{StaticResource Shake}" TargetControl="{Binding #MyBox}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<Border Name="MyBox" Background="Red" Width="50" Height="50" />
```
