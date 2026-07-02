# InvokeCommandBehaviorBase

`InvokeCommandBehaviorBase` is a base class for behaviors that invoke an `ICommand`. It is similar to `InvokeCommandActionBase` but designed for behaviors rather than actions.

`CanExecuteCommand` is implemented with the public `CommandCanExecuteObserver` helper, which custom behaviors can also use to expose their own bindable command availability state.

## Properties

*   **`Command`**: The `ICommand` to execute.
*   **`CanExecuteCommand`**: A read-only value that observes `Command.CanExecute(CommandParameter)`.
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

Use `CanExecuteCommand` when a behavior is attached to a control that does not have its own command source state:

```xml
<Border IsEnabled="{Binding #DropBehavior.CanExecuteCommand}">
    <Interaction.Behaviors>
        <FilesDropBehavior x:Name="DropBehavior"
                           Command="{Binding DropFilesCommand}"
                           CommandParameter="{Binding CurrentFolder}" />
    </Interaction.Behaviors>
</Border>
```
