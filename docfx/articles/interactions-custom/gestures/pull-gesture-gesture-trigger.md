# PullGestureGestureTrigger

A trigger that executes when a pull gesture is detected on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PullGestureGestureTrigger>
            <InvokeCommandAction Command="{Binding PullCommand}" />
        </PullGestureGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Pull Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
