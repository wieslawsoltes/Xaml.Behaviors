# AttachedToVisualTreeTrigger

The `AttachedToVisualTreeTrigger` executes its actions when the associated object is attached to the visual tree.

### Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <AttachedToVisualTreeTrigger>
            <InvokeCommandAction Command="{Binding OnVisualAttachedCommand}" />
        </AttachedToVisualTreeTrigger>
    </Interaction.Behaviors>
</UserControl>
```
