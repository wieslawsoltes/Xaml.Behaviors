# HoldingGestureTrigger

A trigger that executes when a holding gesture is recognized on the associated object.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <HoldingGestureTrigger>
            <InvokeCommandAction Command="{Binding HoldCommand}" />
        </HoldingGestureTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Hold Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
