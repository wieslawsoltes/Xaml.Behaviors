# InvokeCommandBehaviorBase

`InvokeCommandBehaviorBase` is a base class for behaviors that invoke an `ICommand`. It is similar to `InvokeCommandActionBase` but designed for behaviors rather than actions.

## Properties

*   **`Command`**: The `ICommand` to execute.
*   **`CommandParameter`**: An optional parameter to pass to the command's `Execute` method.
*   **`InputConverter`**: An optional `IValueConverter` to convert the parameter before passing it to the command.
*   **`InputConverterParameter`**: An optional parameter to pass to the `InputConverter`.
*   **`InputConverterLanguage`**: An optional language string to pass to the `InputConverter`.
*   **`PassEventArgsToCommand`**: A boolean property. If `true`, the event arguments are passed to the command.

## Usage

This class simplifies the creation of behaviors that trigger commands.

```csharp
public class MyCommandBehavior : InvokeCommandBehaviorBase
{
    // ... implementation ...
}
```
