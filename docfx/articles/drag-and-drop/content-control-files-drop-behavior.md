# ContentControlFilesDropBehavior

`ContentControlFilesDropBehavior` is a behavior that handles file drop operations specifically for `ContentControl`. It allows you to define visual feedback during the drag operation by changing the content or background of the control.

## Properties

*   **`ContentDuringDrag`**: The content to display in the `ContentControl` when files are being dragged over it.
*   **`BackgroundDuringDrag`**: The background brush to apply to the `ContentControl` when files are being dragged over it.
*   **`Command`**: The `ICommand` to execute when files are dropped. The command parameter will be an array of file path strings (`string[]`) or `IEnumerable<IStorageItem>` depending on the platform.

## Usage

```xml
<ContentControl>
    <Interaction.Behaviors>
        <ContentControlFilesDropBehavior 
            ContentDuringDrag="Drop files here!"
            BackgroundDuringDrag="LightGray"
            Command="{Binding DropFilesCommand}" />
    </Interaction.Behaviors>
</ContentControl>
```
