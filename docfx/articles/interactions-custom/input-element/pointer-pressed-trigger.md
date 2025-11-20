# PointerPressedTrigger

A trigger that executes when a pointer button is pressed on the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerPressedTrigger>
            <InvokeCommandAction Command="{Binding PointerPressedCommand}" />
        </PointerPressedTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Press Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
