# KeyDownTrigger

A trigger that executes when a key is pressed while the associated `InputElement` has focus.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Key | `Key?` | Gets or sets the key that triggers the action. |
| Gesture | `KeyGesture?` | Gets or sets the key gesture that triggers the action. |

## Usage

```xml
<TextBox Width="200">
    <Interaction.Behaviors>
        <KeyDownTrigger Key="Enter">
            <InvokeCommandAction Command="{Binding EnterPressedCommand}" />
        </KeyDownTrigger>
    </Interaction.Behaviors>
</TextBox>
```
