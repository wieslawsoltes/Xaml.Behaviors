# KeyTrigger

A trigger that listens for key events and executes its actions when the specified key or gesture is detected.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Key | `Key?` | Gets or sets the key that triggers the action. |
| Gesture | `KeyGesture?` | Gets or sets the key gesture that triggers the action. |
| Event | `FiredOn` | Specifies which keyboard event will fire the trigger (`KeyDown` or `KeyUp`). Default is `KeyDown`. |

## Usage

```xml
<TextBox Width="200">
    <Interaction.Behaviors>
        <KeyTrigger Key="Escape" Event="KeyUp">
            <InvokeCommandAction Command="{Binding CancelCommand}" />
        </KeyTrigger>
    </Interaction.Behaviors>
</TextBox>
```
