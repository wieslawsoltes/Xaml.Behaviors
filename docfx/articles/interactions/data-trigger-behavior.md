# DataTriggerBehavior

When a string `Value` cannot be converted to the runtime type of `Binding`, the comparison uses non-equal semantics instead of throwing: `NotEqual` evaluates to `true`, while the other comparison operators evaluate to `false`.

`DataTriggerBehavior` is a behavior that listens for changes to a bound value and invokes actions when the value meets a specified condition.

## Properties

*   **`Binding`**: The binding to monitor.
*   **`ComparisonCondition`**: The type of comparison to perform (e.g., `Equal`, `NotEqual`, `GreaterThan`).
*   **`Value`**: The value to compare against.

## Usage

```xml
<TextBlock Text="Status">
    <Interaction.Behaviors>
        <DataTriggerBehavior Binding="{Binding IsActive}" ComparisonCondition="Equal" Value="True">
            <ChangePropertyAction PropertyName="Foreground" Value="Green" />
        </DataTriggerBehavior>
        <DataTriggerBehavior Binding="{Binding IsActive}" ComparisonCondition="Equal" Value="False">
            <ChangePropertyAction PropertyName="Foreground" Value="Red" />
        </DataTriggerBehavior>
    </Interaction.Behaviors>
</TextBlock>
```

For `Equal` and `NotEqual` comparisons, `null` is a valid bound value. Relational comparisons with `null` remain inactive, and an omitted `Binding` is not treated as a bound `null` value.

```xml
<DataTriggerBehavior Binding="{Binding SelectedItem}"
                     ComparisonCondition="Equal"
                     Value="{x:Null}">
    <ChangePropertyAction PropertyName="Text" Value="No item selected" />
</DataTriggerBehavior>
```

## DataTrigger

`DataTrigger` is a deprecated class that functions similarly to `DataTriggerBehavior` but inherits from `Trigger`. It is recommended to use `DataTriggerBehavior` instead.
