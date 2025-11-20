# PointerWheelChangedTrigger

A trigger that executes when the mouse wheel is scrolled on the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerWheelChangedTrigger>
            <InvokeCommandAction Command="{Binding WheelChangedCommand}" />
        </PointerWheelChangedTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Scroll Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
