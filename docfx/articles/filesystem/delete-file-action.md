# DeleteFileAction

The `DeleteFileAction` is an action that deletes a file at the specified path.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Path` | `string` | Gets or sets the path of the file to delete. |

## Usage

The following example demonstrates how to use `DeleteFileAction` to delete a file when a button is clicked.

```xml
<Button Content="Delete File">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <DeleteFileAction Path="{Binding TargetPath, StringFormat='{}{0}/test.txt'}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
