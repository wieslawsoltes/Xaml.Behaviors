# ExecuteCommandOnKeyUpBehavior

A behavior that executes a command when a key is released.

## Usage

```xml
<TextBox xmlns="https://github.com/avaloniaui"
         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <ExecuteCommandOnKeyUpBehavior Key="Enter" Command="{Binding EnterCommand}" />
    </Interaction.Behaviors>
</TextBox>
```

