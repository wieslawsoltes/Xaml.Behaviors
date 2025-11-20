# ScrollGestureGestureTrigger

A trigger that executes when a scroll gesture is detected on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <ScrollGestureGestureTrigger>
            <InvokeCommandAction Command="{Binding ScrollCommand}" />
        </ScrollGestureGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Scroll Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
