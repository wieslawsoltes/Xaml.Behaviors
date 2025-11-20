# WriteTextToFileAction

The `WriteTextToFileAction` is an action that writes text to a file.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Path` | `string` | Gets or sets the path of the file. |
| `Text` | `string` | Gets or sets the text to write. |
| `Append` | `bool` | Gets or sets a value indicating whether to append text to the file. Default is `false`. |

## Usage

The following example demonstrates how to use `WriteTextToFileAction` to write text to a file when a button is clicked.

```xml
<Button Content="Write Text">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <WriteTextToFileAction Path="{Binding TargetPath, StringFormat='{}{0}/test.txt'}" 
                                   Text="{Binding FileContent}" 
                                   Append="False" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
