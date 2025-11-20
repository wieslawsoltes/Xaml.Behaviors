# ExecuteCommandOnLostFocusBehavior

A behavior that executes a command when the control loses focus.

## Usage

```xml
<TextBox xmlns="https://github.com/avaloniaui"
         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <ExecuteCommandOnLostFocusBehavior Command="{Binding LostFocusCommand}" />
    </Interaction.Behaviors>
</TextBox>
```

