# KeyUpTrigger

A trigger that executes when a key is released while the associated `InputElement` has focus.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Key | `Key?` | Gets or sets the key that triggers the action. |
| Gesture | `KeyGesture?` | Gets or sets the key gesture that triggers the action. |

## Usage

```xml
<TextBox Width="200">
    <Interaction.Behaviors>
        <KeyUpTrigger Key="Enter">
            <InvokeCommandAction Command="{Binding EnterReleasedCommand}" />
        </KeyUpTrigger>
    </Interaction.Behaviors>
</TextBox>
```
