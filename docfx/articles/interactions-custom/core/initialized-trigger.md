# InitializedTrigger

The `InitializedTrigger` executes its actions when the `Initialized` event is raised on the associated `StyledElement`.

### Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <InitializedTrigger>
            <InvokeCommandAction Command="{Binding OnInitializedCommand}" />
        </InitializedTrigger>
    </Interaction.Behaviors>
</UserControl>
```
