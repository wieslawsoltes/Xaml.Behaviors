# PointerReleasedTrigger

A trigger that executes when a pointer button is released on the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerReleasedTrigger>
            <InvokeCommandAction Command="{Binding PointerReleasedCommand}" />
        </PointerReleasedTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Release Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
