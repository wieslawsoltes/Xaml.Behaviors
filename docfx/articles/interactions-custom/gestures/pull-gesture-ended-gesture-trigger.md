# PullGestureEndedGestureTrigger

A trigger that executes when a pull gesture ends on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PullGestureEndedGestureTrigger>
            <InvokeCommandAction Command="{Binding PullEndedCommand}" />
        </PullGestureEndedGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Pull Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
