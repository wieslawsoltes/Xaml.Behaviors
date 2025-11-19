# MultiDataTriggerBehavior

`MultiDataTriggerBehavior` is a behavior that applies actions when a set of conditions are met. It allows you to define multiple conditions that must all be satisfied.

## Usage

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <MultiDataTriggerBehavior>
            <MultiDataTriggerBehavior.Conditions>
                <Condition Binding="{Binding IsEnabled}" ComparisonCondition="Equal" Value="True" />
                <Condition Binding="{Binding IsVisible}" ComparisonCondition="Equal" Value="True" />
            </MultiDataTriggerBehavior.Conditions>
            <ChangePropertyAction PropertyName="Background" Value="Green" />
        </MultiDataTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

## Conditions

The `MultiDataTriggerBehavior` uses a collection of `Condition` objects to determine if the actions should be executed. Each `Condition` has the following properties:

*   **`Binding`**: The binding to evaluate.
*   **`ComparisonCondition`**: The type of comparison to perform (e.g., `Equal`, `NotEqual`, `LessThan`, `GreaterThan`, etc.).
*   **`Value`**: The value to compare against.

All conditions in the collection must evaluate to true for the actions to be invoked.

## MultiDataTrigger

`MultiDataTrigger` is a deprecated class that functions similarly to `MultiDataTriggerBehavior` but inherits from `Trigger`. It is recommended to use `MultiDataTriggerBehavior` instead.
