# PointerCaptureLostTrigger

A trigger that executes when the associated `InputElement` loses pointer capture.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <PointerCaptureLostTrigger>
            <InvokeCommandAction Command="{Binding PointerCaptureLostCommand}" />
        </PointerCaptureLostTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Capture Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
