# ShowPopupAction

Opens an existing `Popup` control.

```xml
<Button Content="Open Popup">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowPopupAction Popup="{Binding #MyPopup}" TargetControl="{Binding $self}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
