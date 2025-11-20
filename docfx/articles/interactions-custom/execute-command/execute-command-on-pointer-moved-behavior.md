# ExecuteCommandOnPointerMovedBehavior

A behavior that executes a command when the pointer moves over the control.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Pointer Moved">
    <Interaction.Behaviors>
        <ExecuteCommandOnPointerMovedBehavior Command="{Binding PointerMovedCommand}" />
    </Interaction.Behaviors>
</Button>
```

