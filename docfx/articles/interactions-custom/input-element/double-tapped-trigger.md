# DoubleTappedTrigger

A trigger that executes when a double tap occurs on the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <DoubleTappedTrigger>
            <InvokeCommandAction Command="{Binding DoubleTapCommand}" />
        </DoubleTappedTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Double Tap Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
