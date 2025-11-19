# Conditions

The `Condition` class allows you to define logic that determines whether a trigger should fire or an action should execute. It is a core component of the Interactivity SDK, primarily used by `DataTriggerBehavior` and `MultiDataTriggerBehavior` (in the Interactions package), but available for use in custom behaviors and triggers as well.

## Properties

*   **`Binding`**: The binding to evaluate.
*   **`ComparisonCondition`**: The type of comparison to perform (e.g., `Equal`, `NotEqual`, `GreaterThan`).
*   **`Value`**: The value to compare against.
*   **`Property`**: The `AvaloniaProperty` to monitor (alternative to `Binding`).
*   **`SourceName`**: The name of the element that supplies the `Property`.

## ComparisonConditionType

The `ComparisonConditionType` enum defines the available comparison operators:

*   `Equal`
*   `NotEqual`
*   `LessThan`
*   `LessThanOrEqual`
*   `GreaterThan`
*   `GreaterThanOrEqual`

## Usage

### In XAML

Conditions are typically used within a `DataTriggerBehavior`.

```xml
<DataTriggerBehavior Binding="{Binding IsEnabled}" ComparisonCondition="Equal" Value="True">
    <!-- Actions -->
</DataTriggerBehavior>
```

### In Custom Code

You can use `Condition` in your own behaviors to encapsulate comparison logic.

```csharp
var condition = new Condition
{
    Binding = new Binding("Count"),
    ComparisonCondition = ComparisonConditionType.GreaterThan,
    Value = 10
};

// ... later ...
if (condition.Evaluate(dataContext))
{
    // Do something
}
```

## Evaluation Logic

The `Condition` class handles the complexity of comparing different types (e.g., converting strings to numbers if necessary) to ensure robust comparisons.
