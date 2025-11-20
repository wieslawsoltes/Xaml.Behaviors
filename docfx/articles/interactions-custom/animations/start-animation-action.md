# StartAnimationAction

Starts an animation on the control that initiated the action (the `sender`). This is often the associated object of the behavior invoking the action.

### Example

```xml
<Button Content="Animate Me">
    <Button.Resources>
        <Animation x:Key="Rotate" Duration="0:0:1">
            <KeyFrame Cue="0%">
                <Setter Property="RotateTransform.Angle" Value="0"/>
            </KeyFrame>
            <KeyFrame Cue="100%">
                <Setter Property="RotateTransform.Angle" Value="360"/>
            </KeyFrame>
        </Animation>
    </Button.Resources>

    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <StartAnimationAction Animation="{StaticResource Rotate}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
