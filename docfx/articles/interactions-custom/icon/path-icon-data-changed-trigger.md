# PathIconDataChangedTrigger

Executes actions when the `Data` property of the associated `PathIcon` changes.

### Usage Example

```xml
<PathIcon>
    <Interaction.Behaviors>
        <PathIconDataChangedTrigger>
            <InvokeCommandAction Command="{Binding IconChangedCommand}" />
        </PathIconDataChangedTrigger>
    </Interaction.Behaviors>
</PathIcon>
```
