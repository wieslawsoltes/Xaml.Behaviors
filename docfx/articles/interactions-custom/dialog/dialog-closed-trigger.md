# DialogClosedTrigger

Executes actions when the specified `SourceObject` (a `Window`) raises the `Closed` event.

### Properties
- `SourceObject`: The `Window` to monitor.

### Usage Example

```xml
<Window x:Name="MyDialog">
    <Interaction.Behaviors>
        <DialogOpenedTrigger SourceObject="{Binding ElementName=MyDialog}">
            <InvokeCommandAction Command="{Binding DialogOpenedCommand}" />
        </DialogOpenedTrigger>
        <DialogClosedTrigger SourceObject="{Binding ElementName=MyDialog}">
            <InvokeCommandAction Command="{Binding DialogClosedCommand}" />
        </DialogClosedTrigger>
    </Interaction.Behaviors>
</Window>
```
