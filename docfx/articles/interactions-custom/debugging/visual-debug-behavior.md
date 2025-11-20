# VisualDebugBehavior

The `VisualDebugBehavior` visualizes events on the attached control for debugging purposes. When the specified event fires, it draws a temporary colored border and overlay on the control using the `AdornerLayer`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| EventName | string | The name of the event to listen for. |
| HighlightColor | Color | The color used to highlight the control. Default is Red. |
| Duration | TimeSpan | The duration of the highlight. Default is 0.5 seconds. |

## Usage

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <VisualDebugBehavior EventName="Click" HighlightColor="Red" Duration="0:0:0.5" />
    </Interaction.Behaviors>
</Button>
```

## Remarks

This behavior requires an `AdornerLayer` to be present in the visual tree. Most standard Avalonia containers (like `Window`) provide an `AdornerLayer`.
