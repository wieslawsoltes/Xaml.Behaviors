# MoveElementToPanelAction

Moves the associated element (or the sender) to a specified `Panel`.

### Properties

- `TargetPanel`: The `Panel` to move the element into. If not specified, the action attempts to use the sender as the target panel (if applicable, though typically the sender is the element being moved).

### Usage Example

```xml
<Button Content="Move Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <MoveElementToPanelAction TargetPanel="{Binding ElementName=DestinationPanel}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<StackPanel x:Name="DestinationPanel" />
```
