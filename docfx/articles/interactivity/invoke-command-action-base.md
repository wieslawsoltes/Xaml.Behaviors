# InvokeCommandActionBase

`InvokeCommandActionBase` is an abstract base class for actions that invoke an `ICommand`. It provides a standard set of properties for binding commands and command parameters, making it easier to create actions that interact with ViewModels.

`CanExecuteCommand` is implemented with the public `CommandCanExecuteObserver` helper, which custom actions can also use through this base class.

## Properties

*   **`Command`**: The `ICommand` to execute.
*   **`CanExecuteCommand`**: A read-only value that observes `Command.CanExecute(CommandParameter)`.
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

Use `CanExecuteCommand` when an action is hosted by a trigger and the associated control should reflect command availability:

```xml
<Border IsEnabled="{Binding #OpenAction.CanExecuteCommand}">
    <Interaction.Behaviors>
        <ClickEventTrigger>
            <OpenFilePickerAction x:Name="OpenAction"
                                  Command="{Binding OpenFilesCommand}" />
        </ClickEventTrigger>
    </Interaction.Behaviors>
</Border>
```
