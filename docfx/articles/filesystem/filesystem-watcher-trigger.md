# FileSystemWatcherTrigger

The `FileSystemWatcherTrigger` is a trigger that listens to file system events (Changed, Created, Deleted, Renamed) in a specified path.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Path` | `string` | Gets or sets the path to watch. |
| `Filter` | `string` | Gets or sets the filter string used to determine what files are monitored in a directory. Default is `*.*`. |
| `IncludeSubdirectories` | `bool` | Gets or sets a value indicating whether subdirectories within the specified path should be monitored. Default is `false`. |

## Events

The trigger listens to the following events from `FileSystemWatcher`:
- `Changed`
- `Created`
- `Deleted`
- `Renamed`

When an event occurs, the trigger executes its actions. If the action supports passing a parameter (like `InvokeCommandAction` with `PassEventArgsToCommand="True"`), the `FileSystemEventArgs` or `RenamedEventArgs` will be passed as the parameter.

## Usage

The following example demonstrates how to use `FileSystemWatcherTrigger` to log file system events.

```xml
<FileSystemWatcherTrigger Path="{Binding TargetPath}" IncludeSubdirectories="True">
    <core:InvokeCommandAction Command="{Binding LogEventCommand}" PassEventArgsToCommand="True" />
</FileSystemWatcherTrigger>
```
