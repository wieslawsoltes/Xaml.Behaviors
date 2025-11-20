# ConditionalBehavior

`ConditionalBehavior` allows you to attach behaviors conditionally based on a boolean value.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Condition | bool | Gets or sets the condition to check. |
| TrueBehavior | Behavior | Gets or sets the behavior to execute when condition is true. |
| FalseBehavior | Behavior | Gets or sets the behavior to execute when condition is false. |

## Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <ConditionalBehavior Condition="{Binding IsEnabled}">
            <ConditionalBehavior.TrueBehavior>
                <ChangePropertyAction PropertyName="Background" Value="Green" />
            </ConditionalBehavior.TrueBehavior>
            <ConditionalBehavior.FalseBehavior>
                <ChangePropertyAction PropertyName="Background" Value="Red" />
            </ConditionalBehavior.FalseBehavior>
        </ConditionalBehavior>
    </Interaction.Behaviors>
</UserControl>
```
