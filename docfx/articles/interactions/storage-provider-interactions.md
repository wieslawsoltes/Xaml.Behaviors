# StorageProvider Interactions

The `Xaml.Behaviors.Interactions` package provides a set of actions and behaviors for interacting with the `StorageProvider` (File and Folder Pickers) in Avalonia.

## Actions

These actions can be attached to any event trigger and will open the corresponding picker when executed.

*   **`OpenFilePickerAction`**: Opens a file picker dialog for opening files.
*   **`OpenFolderPickerAction`**: Opens a folder picker dialog.
*   **`SaveFilePickerAction`**: Opens a file picker dialog for saving a file.

### Usage Example

```xml
<Button Content="Open File">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <OpenFilePickerAction Title="Select a file" AllowMultiple="True" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

## Behaviors

For convenience, specialized behaviors are provided for common controls like `Button` and `MenuItem`. These behaviors automatically handle the `Click` event.

### Button Behaviors

*   **`ButtonOpenFilePickerBehavior`**
*   **`ButtonOpenFolderPickerBehavior`**
*   **`ButtonSaveFilePickerBehavior`**

```xml
<Button Content="Open File">
    <Interaction.Behaviors>
        <ButtonOpenFilePickerBehavior Title="Select a file" />
    </Interaction.Behaviors>
</Button>
```

### MenuItem Behaviors

*   **`MenuItemOpenFilePickerBehavior`**
*   **`MenuItemOpenFolderPickerBehavior`**
*   **`MenuItemSaveFilePickerBehavior`**

```xml
<MenuItem Header="Open File">
    <Interaction.Behaviors>
        <MenuItemOpenFilePickerBehavior Title="Select a file" />
    </Interaction.Behaviors>
</MenuItem>
```

## Properties

Common properties available on these actions and behaviors include:

*   **`Title`**: The title of the picker dialog.
*   **`AllowMultiple`**: (Open File only) Whether to allow selecting multiple files.
*   **`FileTypeFilter`**: A collection of file types to filter by.
*   **`SuggestedStartLocation`**: The initial location for the picker.
