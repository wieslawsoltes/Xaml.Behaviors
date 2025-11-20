# ContextDialogOpenedTrigger

Fires when the associated `ContextDialogBehavior` opens.

### Example

```xml
<ContextDialogBehavior>
    <Interaction.Behaviors>
        <ContextDialogOpenedTrigger>
            <CallMethodAction TargetObject="{Binding}" MethodName="OnDialogOpened" />
        </ContextDialogOpenedTrigger>
    </Interaction.Behaviors>
</ContextDialogBehavior>
```
