# HidePopupAction

Closes an existing `Popup` control.

```xml
<Button Content="Close Popup">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <HidePopupAction Popup="{Binding #MyPopup}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
