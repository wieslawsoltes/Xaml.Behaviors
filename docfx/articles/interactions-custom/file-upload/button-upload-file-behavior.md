# ButtonUploadFileBehavior

A behavior that attaches to a `Button` and initiates a file upload when the button is clicked.

### Properties
- `FilePath`: The local path of the file to upload.
- `Url`: The destination URL for the upload.
- `Command`: An `ICommand` to execute when the upload completes.
- `CommandParameter`: A parameter to pass to the command.

### Usage Example

```xml
<Button Content="Upload">
    <Interaction.Behaviors>
        <ButtonUploadFileBehavior FilePath="/path/to/file.txt" 
                                  Url="https://example.com/upload"
                                  Command="{Binding UploadFinishedCommand}" />
    </Interaction.Behaviors>
</Button>
```
