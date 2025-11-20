# ExecuteCommandOnRightTappedBehavior

A behavior that executes a command when the control is right tapped.

## Usage

```xml
<Button xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Content="Right Tap Me">
    <Interaction.Behaviors>
        <ExecuteCommandOnRightTappedBehavior Command="{Binding RightTappedCommand}" />
    </Interaction.Behaviors>
</Button>
```

