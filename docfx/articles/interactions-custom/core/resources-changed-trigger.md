# ResourcesChangedTrigger

The `ResourcesChangedTrigger` executes its actions when the `ResourcesChanged` event is raised on the associated `StyledElement`.

### Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <ResourcesChangedTrigger>
            <InvokeCommandAction Command="{Binding ResourcesChangedCommand}" />
        </ResourcesChangedTrigger>
    </Interaction.Behaviors>
</UserControl>
```
