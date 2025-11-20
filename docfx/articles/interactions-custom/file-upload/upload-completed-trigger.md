# UploadCompletedTrigger

A trigger that executes actions when its `IsCompleted` property becomes `true`. This is useful for chaining actions after an upload (or any boolean state) completes.

### Properties
- `IsCompleted`: A boolean value indicating completion.

### Usage Example

```xml
<UserControl>
    <Interaction.Behaviors>
        <UploadCompletedTrigger IsCompleted="{Binding IsUploadFinished}">
            <InvokeCommandAction Command="{Binding NotifyUserCommand}" />
        </UploadCompletedTrigger>
    </Interaction.Behaviors>
</UserControl>
```
