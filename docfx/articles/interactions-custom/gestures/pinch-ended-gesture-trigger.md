# PinchEndedGestureTrigger

A trigger that executes when a pinch gesture ends on the associated object.

## Usage

```xml
<Image Source="image.png" Width="200" Height="200">
    <Interaction.Behaviors>
        <PinchEndedGestureTrigger>
            <InvokeCommandAction Command="{Binding PinchEndedCommand}" />
        </PinchEndedGestureTrigger>
    </Interaction.Behaviors>
</Image>
```
