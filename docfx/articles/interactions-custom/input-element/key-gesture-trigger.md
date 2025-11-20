# KeyGestureTrigger

A trigger that executes when a specific key gesture is detected.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Gesture | `KeyGesture?` | Gets or sets the gesture that will fire the trigger. |
| FiredOn | `KeyGestureTriggerFiredOn` | Gets or sets whether the trigger reacts on key down or key up. Default is `KeyDown`. |

## Usage

```xml
<Window>
    <Interaction.Behaviors>
        <KeyGestureTrigger Gesture="Ctrl+S">
            <InvokeCommandAction Command="{Binding SaveCommand}" />
        </KeyGestureTrigger>
    </Interaction.Behaviors>
</Window>
```
