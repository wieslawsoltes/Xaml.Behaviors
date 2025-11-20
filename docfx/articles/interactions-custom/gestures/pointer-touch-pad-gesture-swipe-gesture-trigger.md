# PointerTouchPadGestureSwipeGestureTrigger

A trigger that executes when a touch pad swipe gesture is detected on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerTouchPadGestureSwipeGestureTrigger>
            <InvokeCommandAction Command="{Binding SwipeCommand}" />
        </PointerTouchPadGestureSwipeGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Swipe Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
