# ContextDialogClosedTrigger

Fires when the associated `ContextDialogBehavior` closes.

### Example

```xml
<ContextDialogBehavior>
    <Interaction.Behaviors>
        <ContextDialogClosedTrigger>
            <CallMethodAction TargetObject="{Binding}" MethodName="OnDialogClosed" />
        </ContextDialogClosedTrigger>
    </Interaction.Behaviors>
</ContextDialogBehavior>
```
