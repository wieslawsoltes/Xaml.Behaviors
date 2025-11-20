# ScrollGestureEndedGestureTrigger

A trigger that executes when a scroll gesture ends on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <ScrollGestureEndedGestureTrigger>
            <InvokeCommandAction Command="{Binding ScrollEndedCommand}" />
        </ScrollGestureEndedGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Scroll Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
