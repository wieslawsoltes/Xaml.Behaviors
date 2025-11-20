# ExecuteCommandOnGotFocusBehavior

A behavior that executes a command when the control gets focus.

## Usage

```xml
<TextBox xmlns="https://github.com/avaloniaui"
         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <ExecuteCommandOnGotFocusBehavior Command="{Binding GotFocusCommand}" />
    </Interaction.Behaviors>
</TextBox>
```

