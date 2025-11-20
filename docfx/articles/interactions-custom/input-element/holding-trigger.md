# HoldingTrigger

A trigger that executes when a holding gesture occurs on the associated `InputElement`.

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <HoldingTrigger>
            <InvokeCommandAction Command="{Binding HoldCommand}" />
        </HoldingTrigger>
    </Interaction.Behaviors>
    <TextBlock Text="Hold Me" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```
