# ExecuteCommandOnTextInputMethodClientRequestedBehavior

A behavior that executes a command when the text input method client is requested.

## Usage

```xml
<TextBox xmlns="https://github.com/avaloniaui"
         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <ExecuteCommandOnTextInputMethodClientRequestedBehavior Command="{Binding TextInputMethodClientRequestedCommand}" />
    </Interaction.Behaviors>
</TextBox>
```

