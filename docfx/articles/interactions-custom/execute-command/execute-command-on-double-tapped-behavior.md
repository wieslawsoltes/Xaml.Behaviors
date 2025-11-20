# ExecuteCommandOnDoubleTappedBehavior

A behavior that executes a command when the control is double tapped.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Double Tap Me">
    <Interaction.Behaviors>
        <ExecuteCommandOnDoubleTappedBehavior Command="{Binding DoubleTappedCommand}" />
    </Interaction.Behaviors>
</Button>
```

