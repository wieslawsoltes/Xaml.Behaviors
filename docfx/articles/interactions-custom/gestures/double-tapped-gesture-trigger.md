# DoubleTappedGestureTrigger

A trigger that executes when a double tap gesture is recognized on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <DoubleTappedGestureTrigger>
            <InvokeCommandAction Command="{Binding DoubleTapCommand}" />
        </DoubleTappedGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Double Tap Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
