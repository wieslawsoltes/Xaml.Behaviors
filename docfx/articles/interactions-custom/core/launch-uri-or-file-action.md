# LaunchUriOrFileAction

The `LaunchUriOrFileAction` opens a file or URI using the default system application.

### Properties
- `File`: The file path or URI to open.

### Usage Example

```xml
<Button Content="Open Website">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <LaunchUriOrFileAction File="https://www.avaloniaui.net" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
