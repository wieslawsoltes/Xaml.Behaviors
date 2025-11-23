# Typed MultiDataTrigger

The `[GenerateTypedMultiDataTrigger]` attribute generates a strongly-typed `Trigger` that evaluates multiple conditions. This provides better performance than the standard `MultiDataTriggerBehavior` by avoiding boxing, reflection, and runtime type checking.

## Usage

Apply the `[GenerateTypedMultiDataTrigger]` attribute to a partial class inheriting from `StyledElementTrigger`. Use the `[TriggerProperty]` attribute on fields to define the Avalonia properties that will be used in the condition.

```csharp
using Avalonia.Xaml.Interactivity;
using Xaml.Behaviors.SourceGenerators;

namespace MyApp.Behaviors;

[GenerateTypedMultiDataTrigger]
public partial class ValidationTrigger : StyledElementTrigger
{
    [TriggerProperty]
    private bool _isValid;

    [TriggerProperty]
    private int _retryCount;

    // Implement the evaluation logic
    private bool Evaluate()
    {
        // This logic is compiled directly, ensuring type safety and performance
        return IsValid && RetryCount < 3;
    }
}
```

> Note: The target type must be a top-level `partial` class (nested types are not supported for this generator).
> The `Evaluate` method must be a non-generic, parameterless method returning `bool`.
> The generated class is `public` unless the target or any `[TriggerProperty]` field type requires `internal`.

## Generated Code

The source generator creates:
1.  Avalonia properties for each field marked with `[TriggerProperty]` (e.g., `IsValidProperty`, `RetryCountProperty`).
2.  Public properties wrapping the Avalonia properties.
3.  An `OnPropertyChanged` override that updates the backing fields and calls `Evaluate()`.
4.  Logic to execute the actions if `Evaluate()` returns `true`.

## XAML Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ac="using:Avalonia.Controls"
             xmlns:local="using:MyApp.Behaviors">
    <TextBlock Text="Status">
        <Interaction.Behaviors>
            <local:ValidationTrigger IsValid="{Binding IsFormValid}" RetryCount="{Binding Retries}">
                <ac:SetForegroundAction Value="Red" />
            </local:ValidationTrigger>
        </Interaction.Behaviors>
    </TextBlock>
</UserControl>
```

## Benefits

*   **Zero Allocation**: Values are stored in fields of their native type, avoiding boxing.
*   **Performance**: Comparisons are compiled C# code, not runtime interpretation.
*   **Type Safety**: You cannot accidentally compare incompatible types.
