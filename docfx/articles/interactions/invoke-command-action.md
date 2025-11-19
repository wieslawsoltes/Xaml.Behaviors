# InvokeCommandAction

`InvokeCommandAction` is an action that executes an `ICommand` when invoked. It inherits from `InvokeCommandActionBase` and provides standard command execution capabilities.

## Properties

*   **`Command`**: The `ICommand` to execute.
*   **`CommandParameter`**: The parameter to pass to the command.
*   **`InputConverter`**: A converter to process the parameter before passing it to the command.
*   **`InputConverterParameter`**: A parameter for the converter.
*   **`PassEventArgsToCommand`**: If `true`, passes the event arguments (or trigger parameter) to the command.

## Usage

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <InvokeCommandAction Command="{Binding MyCommand}" CommandParameter="Hello" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
