# TappedTrigger

A trigger that executes when a tap occurs on the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <TappedTrigger>
            <InvokeCommandAction Command="{Binding TapCommand}" />
        </TappedTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Tap Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
