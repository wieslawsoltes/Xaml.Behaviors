# SetViewModelPropertyOnLoadBehavior

Sets a view model property when the associated control is loaded.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PropertyName | `string` | Gets or sets the property name to change. |
| Value | `object` | Gets or sets the value to assign. |

## Usage

```xml
<UserControl>
    <Interaction.Behaviors>
        <SetViewModelPropertyOnLoadBehavior PropertyName="IsInitialized" Value="True" />
    </Interaction.Behaviors>
</UserControl>
```
