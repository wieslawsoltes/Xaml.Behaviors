# HideControlAction

Sets the `IsVisible` property of the target control to `False`.

```xml
<Button Content="Close">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <HideControlAction TargetObject="{Binding #DetailsPanel}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
