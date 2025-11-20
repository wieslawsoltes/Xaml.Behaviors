# WindowStateTrigger

Executes actions when the window state matches the specified value.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Window | `Window` | Gets or sets the window. If not set, the visual root window is used. |
| State | `WindowState` | Gets or sets the window state to trigger on. |

## Usage

```xml
<Window>
    <Interaction.Behaviors>
        <WindowStateTrigger State="Minimized">
            <CallMethodAction TargetObject="{Binding}" MethodName="OnMinimized" />
        </WindowStateTrigger>
    </Interaction.Behaviors>
</Window>
```
