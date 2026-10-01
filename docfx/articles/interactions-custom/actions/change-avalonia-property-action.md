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

### Uno Platform

WinUI dependency properties do not expose their type, so the Uno Platform version converts string values to a type
inferred from the default value or the current value of the property (only strings are converted when the type is
inferred; strings that do not convert are assigned as is). Set the Uno-only **TargetPropertyType** when the property
has neither, for example `Border.Background`:

```xml
<icustom:ChangeAvaloniaPropertyAction TargetObject="{x:Bind TargetBorder}"
                                      TargetProperty="{x:Bind controls:Border.BackgroundProperty}"
                                      TargetPropertyType="media:Brush"
                                      Value="Black" />
```
