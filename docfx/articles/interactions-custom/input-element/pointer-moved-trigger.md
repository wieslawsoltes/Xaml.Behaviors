# PointerMovedTrigger

A trigger that executes when the pointer moves over the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerMovedTrigger>
            <InvokeCommandAction Command="{Binding PointerMovedCommand}" />
        </PointerMovedTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Move Over Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
