# HideContextDialogAction

Closes the specified `ContextDialogBehavior`.

### Properties
*   `TargetDialog`: The `ContextDialogBehavior` instance to close.

### Example

```xml
<Button Content="Cancel">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <HideContextDialogAction TargetDialog="{Binding #MyDialog}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
