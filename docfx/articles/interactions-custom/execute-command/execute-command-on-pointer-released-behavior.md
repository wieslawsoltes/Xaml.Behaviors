# ExecuteCommandOnPointerReleasedBehavior

A behavior that executes a command when the pointer is released on the control.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Pointer Released">
    <Interaction.Behaviors>
        <ExecuteCommandOnPointerReleasedBehavior Command="{Binding PointerReleasedCommand}" />
    </Interaction.Behaviors>
</Button>
```

