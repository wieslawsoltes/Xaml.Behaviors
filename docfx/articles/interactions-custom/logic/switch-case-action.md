# SwitchCaseAction

`SwitchCaseAction` executes a specific set of actions based on a value match.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Value | object | Gets or sets the value to switch on. |
| Cases | AvaloniaList&lt;Case&gt; | Gets the collection of cases. |
| DefaultActions | ActionCollection | Gets the actions to execute if no case matches. |

## Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Button Content="Click Me">
        <Interaction.Actions>
            <EventTriggerBehavior EventName="Click">
                <SwitchCaseAction Value="{Binding Status}">
                    <SwitchCaseAction.Cases>
                        <Case Value="Active">
                            <ChangePropertyAction PropertyName="Background" Value="Green" />
                        </Case>
                        <Case Value="Inactive">
                            <ChangePropertyAction PropertyName="Background" Value="Gray" />
                        </Case>
                    </SwitchCaseAction.Cases>
                    <SwitchCaseAction.DefaultActions>
                        <ChangePropertyAction PropertyName="Background" Value="Red" />
                    </SwitchCaseAction.DefaultActions>
                </SwitchCaseAction>
            </EventTriggerBehavior>
        </Interaction.Actions>
    </Button>
</UserControl>
```
