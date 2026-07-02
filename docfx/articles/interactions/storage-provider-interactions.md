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

Picker actions inherit `CanExecuteCommand`, which observes command availability.
This is useful when the picker is opened from a non-button surface such as a panel click trigger:

```xml
<Border IsEnabled="{Binding #OpenPanelPicker.CanExecuteCommand}">
    <Interaction.Behaviors>
        <ClickEventTrigger>
            <OpenFilePickerAction x:Name="OpenPanelPicker"
                                  Title="Select a file"
                                  Command="{Binding OpenFilesCommand}"
                                  CanExecuteCommandParameter="OpenFiles" />
        </ClickEventTrigger>
    </Interaction.Behaviors>
</Border>
```

Picker actions normally execute the command with the picker result. Set `CanExecuteCommandParameter` when the command has a global availability state that should be observed before a picker result exists.

Use `UseCommandCanExecuteForIsEnabled` when the action should automatically bind the trigger's associated control to `CanExecuteCommand`:

```xml
<Border>
    <Interaction.Behaviors>
        <ClickEventTrigger>
            <OpenFilePickerAction Title="Select a file"
                                  Command="{Binding OpenFilesCommand}"
                                  CanExecuteCommandParameter="OpenFiles"
                                  UseCommandCanExecuteForIsEnabled="True" />
        </ClickEventTrigger>
    </Interaction.Behaviors>
</Border>
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

Picker behaviors inherit `CanExecuteCommand`, which observes command availability.
Bind the associated control's enabled state to it when you want command availability to drive the control:

```xml
<Button Content="Open File"
        IsEnabled="{Binding #OpenFilePicker.CanExecuteCommand}">
    <Interaction.Behaviors>
        <ButtonOpenFilePickerBehavior x:Name="OpenFilePicker"
                                      Title="Select a file"
                                      Command="{Binding OpenFilesCommand}"
                                      CanExecuteCommandParameter="OpenFiles" />
    </Interaction.Behaviors>
</Button>
```

Use `UseCommandCanExecuteForIsEnabled` when the behavior should create that binding for the associated control:

```xml
<Button Content="Open File">
    <Interaction.Behaviors>
        <ButtonOpenFilePickerBehavior Title="Select a file"
                                      Command="{Binding OpenFilesCommand}"
                                      CanExecuteCommandParameter="OpenFiles"
                                      UseCommandCanExecuteForIsEnabled="True" />
    </Interaction.Behaviors>
</Button>
```

The sample application includes a `Picker CanExecuteCommand` page that shows all six picker behaviors and a panel-hosted picker action using command `CanExecute` state.

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
*   **`CanExecuteCommand`**: (Actions and behaviors) Whether the configured command can execute with the current can-execute parameter.
*   **`CanExecuteCommandParameter`**: (Actions and behaviors) Optional parameter used only for `Command.CanExecute`. Use this when command execution should still receive the picker result.
*   **`UseCommandCanExecuteForIsEnabled`**: (Actions and behaviors) Whether the associated control should automatically follow `CanExecuteCommand` through `IsEnabled`.
