# DataTriggerBehavior

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

## DataTrigger

`DataTrigger` is a deprecated class that functions similarly to `DataTriggerBehavior` but inherits from `Trigger`. It is recommended to use `DataTriggerBehavior` instead.
