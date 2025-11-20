# DeleteDirectoryAction

The `DeleteDirectoryAction` is an action that deletes a directory at the specified path.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Path` | `string` | Gets or sets the path of the directory to delete. |
| `Recursive` | `bool` | Gets or sets a value indicating whether to delete subdirectories and files. Default is `true`. |

## Usage

The following example demonstrates how to use `DeleteDirectoryAction` to delete a directory recursively when a button is clicked.

```xml
<Button Content="Delete Directory">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <DeleteDirectoryAction Path="{Binding TargetPath}" Recursive="True" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
