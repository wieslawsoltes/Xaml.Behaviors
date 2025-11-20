# ExecuteCommandOnTappedBehavior

A behavior that executes a command when the control is tapped.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Tap Me">
    <Interaction.Behaviors>
        <ExecuteCommandOnTappedBehavior Command="{Binding TappedCommand}" />
    </Interaction.Behaviors>
</Button>
```

