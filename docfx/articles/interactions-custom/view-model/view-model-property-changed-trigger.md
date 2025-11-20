# ViewModelPropertyChangedTrigger

Triggers when the specified view model property changes.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PropertyName | `string` | Gets or sets the name of the property to monitor. |

## Usage

```xml
<UserControl>
    <Interaction.Behaviors>
        <ViewModelPropertyChangedTrigger PropertyName="Status">
            <CallMethodAction TargetObject="{Binding}" MethodName="OnStatusChanged" />
        </ViewModelPropertyChangedTrigger>
    </Interaction.Behaviors>
</UserControl>
```
