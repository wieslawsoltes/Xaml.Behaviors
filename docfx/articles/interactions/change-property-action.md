# ChangePropertyAction

`ChangePropertyAction` is an action that changes the value of a property on a specified object when invoked.

## Properties

*   **`TargetObject`**: The object whose property will be changed. If not set, it defaults to the `AssociatedObject`.
*   **`PropertyName`**: The name of the property to change.
*   **`Value`**: The new value to set.

## Usage

```xml
<Button Content="Hover Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="PointerEnter">
            <ChangePropertyAction PropertyName="Background" Value="Red" />
        </EventTriggerBehavior>
        <EventTriggerBehavior EventName="PointerExited">
            <ChangePropertyAction PropertyName="Background" Value="Blue" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
