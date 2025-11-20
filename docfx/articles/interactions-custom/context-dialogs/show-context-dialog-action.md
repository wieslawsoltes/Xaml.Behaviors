# ShowContextDialogAction

Opens the specified `ContextDialogBehavior`.

### Properties
*   `TargetDialog`: The `ContextDialogBehavior` instance to open.

### Example

```xml
<Button Content="Show Info">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowContextDialogAction TargetDialog="{Binding #InfoDialog}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
