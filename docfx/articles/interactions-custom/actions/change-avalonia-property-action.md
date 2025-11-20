# ChangeAvaloniaPropertyAction

This action is similar to the standard `ChangePropertyAction`, but it is designed specifically for `AvaloniaProperty` objects. It allows you to set a property value using the strongly-typed `AvaloniaProperty` definition rather than a string name.

### Properties

*   **TargetObject**: The object whose property will be changed.
*   **TargetProperty**: The `AvaloniaProperty` to change.
*   **Value**: The new value to set.

### Example

```xml
<Button Content="Change Color">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ChangeAvaloniaPropertyAction TargetObject="{Binding #MyBlock}" 
                                          TargetProperty="{x:Static TextBlock.BackgroundProperty}"
                                          Value="Red" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<TextBlock Name="MyBlock" Text="Hello World" />
```
