# ExecuteCommandOnPointerEnteredBehavior

A behavior that executes a command when the pointer enters the control.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Pointer Entered">
    <Interaction.Behaviors>
        <ExecuteCommandOnPointerEnteredBehavior Command="{Binding PointerEnteredCommand}" />
    </Interaction.Behaviors>
</Button>
```

