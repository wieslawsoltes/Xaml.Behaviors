# PinchGestureTrigger

A trigger that executes when a pinch gesture is detected on the associated object.

## Usage

```xml
<Image Source="image.png" Width="200" Height="200">
    <Interaction.Behaviors>
        <PinchGestureTrigger>
            <InvokeCommandAction Command="{Binding PinchCommand}" />
        </PinchGestureTrigger>
    </Interaction.Behaviors>
</Image>
```
