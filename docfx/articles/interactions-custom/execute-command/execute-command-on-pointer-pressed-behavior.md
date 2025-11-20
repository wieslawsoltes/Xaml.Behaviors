# ExecuteCommandOnPointerPressedBehavior

A behavior that executes a command when the pointer is pressed on the control.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Pointer Pressed">
    <Interaction.Behaviors>
        <ExecuteCommandOnPointerPressedBehavior Command="{Binding PointerPressedCommand}" />
    </Interaction.Behaviors>
</Button>
```

