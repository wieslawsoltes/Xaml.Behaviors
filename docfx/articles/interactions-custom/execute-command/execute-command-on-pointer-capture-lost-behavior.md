# ExecuteCommandOnPointerCaptureLostBehavior

A behavior that executes a command when the pointer capture is lost.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Pointer Capture Lost">
    <Interaction.Behaviors>
        <ExecuteCommandOnPointerCaptureLostBehavior Command="{Binding PointerCaptureLostCommand}" />
    </Interaction.Behaviors>
</Button>
```

