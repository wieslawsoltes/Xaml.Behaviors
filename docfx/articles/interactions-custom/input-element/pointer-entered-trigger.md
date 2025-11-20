# PointerEnteredTrigger

A trigger that executes when the pointer enters the bounds of the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerEnteredTrigger>
            <InvokeCommandAction Command="{Binding PointerEnteredCommand}" />
        </PointerEnteredTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Hover Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
