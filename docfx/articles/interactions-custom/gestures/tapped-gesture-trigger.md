# TappedGestureTrigger

A trigger that executes when a tap gesture is recognized on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <TappedGestureTrigger>
            <InvokeCommandAction Command="{Binding TapCommand}" />
        </TappedGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Tap Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
