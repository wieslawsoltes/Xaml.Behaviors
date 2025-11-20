# PointerTouchPadGestureRotateGestureTrigger

A trigger that executes when a touch pad rotate gesture is detected on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerTouchPadGestureRotateGestureTrigger>
            <InvokeCommandAction Command="{Binding RotateCommand}" />
        </PointerTouchPadGestureRotateGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Rotate Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
