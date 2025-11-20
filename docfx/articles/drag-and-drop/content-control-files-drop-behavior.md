# ContentControlFilesDropBehavior

`ContentControlFilesDropBehavior` is a behavior that handles file drop operations specifically for `ContentControl`. It allows you to define visual feedback during the drag operation by changing the content or background of the control.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| ContentDuringDrag | `object` | Gets or sets the content to display during drag over. |
| BackgroundDuringDrag | `IBrush` | Gets or sets the background to display during drag over. |

## Usage

```xml
<ContentControl>
    <Interaction.Behaviors>
        <ContentControlFilesDropBehavior ContentDuringDrag="Drop Here!" BackgroundDuringDrag="LightGreen" />
    </Interaction.Behaviors>
    <TextBlock Text="Drag files here" />
</ContentControl>
```

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
