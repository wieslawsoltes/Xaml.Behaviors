# FocusControlAction

Sets input focus to a specific control.

```xml
<Button Content="Search">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <FocusControlAction TargetObject="{Binding #SearchBox}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<TextBox Name="SearchBox" />
```
