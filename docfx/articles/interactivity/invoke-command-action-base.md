# InvokeCommandActionBase

`InvokeCommandActionBase` is an abstract base class for actions that invoke an `ICommand`. It provides a standard set of properties for binding commands and command parameters, making it easier to create actions that interact with ViewModels.

## Properties

*   **`Command`**: The `ICommand` to execute.
*   **`CommandParameter`**: An optional parameter to pass to the command's `Execute` method.
*   **`InputConverter`**: An optional `IValueConverter` to convert the parameter before passing it to the command.
*   **`InputConverterParameter`**: An optional parameter to pass to the `InputConverter`.
*   **`InputConverterLanguage`**: An optional language string to pass to the `InputConverter`.
*   **`PassEventArgsToCommand`**: A boolean property. If `true`, the event arguments (or the parameter passed to `Execute`) are passed to the command. If `false` (default), `CommandParameter` is used if set; otherwise, the parameter passed to `Execute` is used.

## Usage

This class is typically used as a base for actions like `InvokeCommandAction` (in the Interactions package).

```csharp
public class MyCommandAction : InvokeCommandActionBase
{
    public override object? Execute(object? sender, object? parameter)
    {
        if (Command?.CanExecute(CommandParameter ?? parameter) == true)
        {
            Command.Execute(CommandParameter ?? parameter);
        }
        return null;
    }
}
```
