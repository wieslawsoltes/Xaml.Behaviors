# ChangePropertyAction

`ChangePropertyAction` is an action that changes the value of a property on a specified object when invoked.

## Properties

*   **`TargetObject`**: The object whose property will be changed. If not set, it defaults to the `AssociatedObject`.
*   **`PropertyName`**: The name of the property to change.
*   **`Value`**: The new value to set.

Attached properties use the `(Owner.Property)` form. Resolution is based on the target object's registered Avalonia properties, so owner type names are not affected by unrelated types with the same short name in other assemblies.

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

For example, the following action assigns the target to the first grid column:

```xml
<ChangePropertyAction TargetObject="{Binding #ContentPresenter}"
                      PropertyName="(Grid.Column)"
                      Value="0" />
```
