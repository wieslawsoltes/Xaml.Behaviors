# AttachedToLogicalTreeTrigger

The `AttachedToLogicalTreeTrigger` executes its actions when the associated object is attached to the logical tree.

### Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <AttachedToLogicalTreeTrigger>
            <InvokeCommandAction Command="{Binding OnAttachedCommand}" />
        </AttachedToLogicalTreeTrigger>
    </Interaction.Behaviors>
</UserControl>
```
