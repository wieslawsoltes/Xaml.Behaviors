# ExecuteCommandOnHoldingBehavior

A behavior that executes a command when the control is held.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Hold Me">
    <Interaction.Behaviors>
        <ExecuteCommandOnHoldingBehavior Command="{Binding HoldingCommand}" />
    </Interaction.Behaviors>
</Button>
```

