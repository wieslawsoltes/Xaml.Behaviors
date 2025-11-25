# Typed InvokeCommandAction

The `[GenerateTypedInvokeCommandAction]` attribute generates a strongly-typed action that executes an `ICommand`. This avoids the overhead of `IValueConverter` and loose typing often associated with `InvokeCommandAction`.

## Usage

Apply the `[GenerateTypedInvokeCommandAction]` attribute to a partial class inheriting from `StyledElementAction`. Use `[ActionCommand]` and `[ActionParameter]` attributes on fields to define the command and parameter properties.

```csharp
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;
using System.Windows.Input;

namespace MyApp.Behaviors;

[GenerateTypedInvokeCommandAction]
public partial class SubmitAction : StyledElementAction
{
    [ActionCommand]
    private ICommand? _command;

    [ActionParameter]
    private string? _commandParameter;
}
```

> Note: The target type must be a top-level `partial` class derived from `StyledElementAction`; nested types are not supported for this generator.
> The generated class is `public` unless the target type or any command/parameter field type requires `internal`.
> At least one field must be annotated with `[ActionCommand]`; the generator will emit an error if none are found.

## Generated Code

The source generator creates:
1.  DependencyProperties for the command and parameter.
2.  An `Execute` method override that:
    *   Syncs the backing fields with the property values.
    *   Checks `CanExecute`.
    *   Calls `Execute` on the command with the strongly-typed parameter.

## UI Thread Dispatching

If the command should run on the UI thread, set `UseDispatcher = true` on the attribute. The generated action will invoke `CanExecute` and `Execute` inside `Dispatcher.UIThread.Post`; the default remains synchronous on the calling thread.

## XAML Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.Behaviors">
    <Button Content="Submit">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <local:SubmitAction Command="{Binding SubmitCommand}" CommandParameter="UserInitiated" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</UserControl>
```

## Benefits

*   **Type Safety**: Ensures the parameter type matches what the command expects (if you use a typed command implementation).
*   **Performance**: Direct execution without intermediate object conversion or reflection.
