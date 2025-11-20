# UploadFileAction

An action that uploads a file to a specified URL.

### Properties
- `FilePath`: The local path of the file to upload.
- `Url`: The destination URL for the upload.
- `Command`: An `ICommand` to execute when the upload completes.
- `CommandParameter`: A parameter to pass to the command.

### Usage Example

```xml
<Button Content="Upload">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <UploadFileAction FilePath="{Binding SelectedFilePath}" 
                              Url="https://example.com/upload"
                              Command="{Binding UploadFinishedCommand}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
