# SetCursorAction

Sets the `Cursor` property on a target `InputElement`.

### Properties
- `Cursor`: The cursor to apply.
- `TargetObject`: The target `InputElement`. If not set, uses the sender.

### Usage Example

```xml
<Button Content="Change Cursor">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <SetCursorAction TargetObject="{Binding ElementName=MyBorder}" Cursor="Wait" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
