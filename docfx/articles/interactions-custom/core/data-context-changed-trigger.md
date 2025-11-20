# DataContextChangedTrigger

The `DataContextChangedTrigger` executes its actions when the `DataContextChanged` event is raised on the associated `StyledElement`.

### Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <DataContextChangedTrigger>
            <InvokeCommandAction Command="{Binding OnDataContextChangedCommand}" />
        </DataContextChangedTrigger>
    </Interaction.Behaviors>
</UserControl>
```
