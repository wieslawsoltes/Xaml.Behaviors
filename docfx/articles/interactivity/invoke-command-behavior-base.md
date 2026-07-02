# InvokeCommandBehaviorBase

`InvokeCommandBehaviorBase` is a base class for behaviors that invoke an `ICommand`. It is similar to `InvokeCommandActionBase` but designed for behaviors rather than actions.

`CanExecuteCommand` is implemented with the public `CommandCanExecuteObserver` helper, which custom behaviors can also use to expose their own bindable command availability state.

## Properties

*   **`Command`**: The `ICommand` to execute.
*   **`CanExecuteCommand`**: A read-only value that observes command availability.
*   **`CanExecuteCommandParameter`**: An optional parameter used only for `Command.CanExecute`.
*   **`UseCommandCanExecuteForIsEnabled`**: When `true`, the associated control's `IsEnabled` property follows `CanExecuteCommand`.
*   **`CommandParameter`**: An optional parameter to pass to the command's `Execute` method.
*   **`InputConverter`**: An optional `IValueConverter` to convert the parameter before passing it to the command.
*   **`InputConverterParameter`**: An optional parameter to pass to the `InputConverter`.
*   **`InputConverterLanguage`**: An optional language string to pass to the `InputConverter`.
*   **`PassEventArgsToCommand`**: A boolean property. If `true`, the event arguments are passed to the command.

`CanExecuteCommand` uses `CanExecuteCommandParameter` when it is set. Otherwise it uses `CommandParameter` when that is set. If neither value is set and the behavior will execute with event args, converted input, picker results, or another runtime parameter, `CanExecuteCommand` stays `true` instead of probing `CanExecute(null)`.

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
                           CanExecuteCommandParameter="{Binding CurrentFolder}"
                           CommandParameter="{Binding CurrentFolder}" />
    </Interaction.Behaviors>
</Border>
```

Set `UseCommandCanExecuteForIsEnabled` when the behavior should create that binding for the associated control:

```xml
<Border>
    <Interaction.Behaviors>
        <FilesDropBehavior Command="{Binding DropFilesCommand}"
                           CanExecuteCommandParameter="{Binding CurrentFolder}"
                           CommandParameter="{Binding CurrentFolder}"
                           UseCommandCanExecuteForIsEnabled="True" />
    </Interaction.Behaviors>
</Border>
```
