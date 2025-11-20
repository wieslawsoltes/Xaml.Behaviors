# ExecuteCommandOnPointerExitedBehavior

A behavior that executes a command when the pointer exits the control.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Pointer Exited">
    <Interaction.Behaviors>
        <ExecuteCommandOnPointerExitedBehavior Command="{Binding PointerExitedCommand}" />
    </Interaction.Behaviors>
</Button>
```

