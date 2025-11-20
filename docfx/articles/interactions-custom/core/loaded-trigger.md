# LoadedTrigger

The `LoadedTrigger` executes its actions when the `Loaded` event is raised on the associated `Control`.

### Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <LoadedTrigger>
            <InvokeCommandAction Command="{Binding OnLoadedCommand}" />
        </LoadedTrigger>
    </Interaction.Behaviors>
</UserControl>
```
