# SwitchCaseBehavior

`SwitchCaseBehavior` executes a specific set of actions based on a value match.

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
    <Interaction.Behaviors>
        <SwitchCaseBehavior Value="{Binding Status}">
            <SwitchCaseBehavior.Cases>
                <Case Value="Active">
                    <ChangePropertyAction PropertyName="Background" Value="Green" />
                </Case>
                <Case Value="Inactive">
                    <ChangePropertyAction PropertyName="Background" Value="Gray" />
                </Case>
            </SwitchCaseBehavior.Cases>
            <SwitchCaseBehavior.DefaultActions>
                <ChangePropertyAction PropertyName="Background" Value="Red" />
            </SwitchCaseBehavior.DefaultActions>
        </SwitchCaseBehavior>
    </Interaction.Behaviors>
</UserControl>
```
