# CreateDirectoryAction

The `CreateDirectoryAction` is an action that creates a directory at the specified path.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Path` | `string` | Gets or sets the path of the directory to create. |

## Usage

The following example demonstrates how to use `CreateDirectoryAction` to create a directory when a button is clicked.

```xml
<Button Content="Create Directory">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <CreateDirectoryAction Path="{Binding TargetPath}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
