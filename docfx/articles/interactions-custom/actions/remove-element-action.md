# RemoveElementAction

Removes the associated object (or target object) from its parent container. This is a destructive action effectively "closing" a view from the visual tree.

```xml
<Button Content="Close Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <RemoveElementAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
