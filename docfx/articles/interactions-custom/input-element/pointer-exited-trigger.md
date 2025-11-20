# PointerExitedTrigger

A trigger that executes when the pointer leaves the bounds of the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerExitedTrigger>
            <InvokeCommandAction Command="{Binding PointerExitedCommand}" />
        </PointerExitedTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Hover Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
