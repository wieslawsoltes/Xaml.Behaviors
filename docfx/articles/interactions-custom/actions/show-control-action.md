# ShowControlAction

Sets the `IsVisible` property of the target control to `True`.

```xml
<Button Content="Show Details">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowControlAction TargetObject="{Binding #DetailsPanel}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
