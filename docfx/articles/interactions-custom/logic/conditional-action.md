# ConditionalAction

`ConditionalAction` executes different collections of actions depending on the specified condition.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Condition | bool | Gets or sets the condition that determines which actions are executed. |
| Actions | ActionCollection | Gets the actions executed when `Condition` evaluates to `true`. |
| ElseActions | ActionCollection | Gets the actions executed when `Condition` evaluates to `false`. |

## Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Button Content="Click Me">
        <Interaction.Actions>
            <EventTriggerBehavior EventName="Click">
                <ConditionalAction Condition="{Binding IsEnabled}">
                    <ConditionalAction.Actions>
                        <ChangePropertyAction PropertyName="Background" Value="Green" />
                    </ConditionalAction.Actions>
                    <ConditionalAction.ElseActions>
                        <ChangePropertyAction PropertyName="Background" Value="Red" />
                    </ConditionalAction.ElseActions>
                </ConditionalAction>
            </EventTriggerBehavior>
        </Interaction.Actions>
    </Button>
</UserControl>
```
