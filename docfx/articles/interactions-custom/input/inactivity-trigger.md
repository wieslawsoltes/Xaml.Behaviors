# InactivityTrigger

A trigger that fires when the user has been inactive (no mouse/keyboard input) for a specified duration.

## Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| Timeout | TimeSpan | Gets or sets the inactivity timeout duration. Default is 5 seconds. |

## Usage

```xml
<StackPanel>
    <Interaction.Behaviors>
        <InactivityTrigger Timeout="00:00:10">
            <LogAction Message="User is inactive!" />
        </InactivityTrigger>
    </Interaction.Behaviors>
</StackPanel>
```
