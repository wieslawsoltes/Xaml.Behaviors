# PointerTouchPadGestureMagnifyGestureTrigger

A trigger that executes when a touch pad magnify gesture is detected on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerTouchPadGestureMagnifyGestureTrigger>
            <InvokeCommandAction Command="{Binding MagnifyCommand}" />
        </PointerTouchPadGestureMagnifyGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Magnify Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
