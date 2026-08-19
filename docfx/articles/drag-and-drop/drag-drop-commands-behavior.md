# DragDropCommandsBehavior

`DragDropCommandsBehavior` maps Drag and Drop events directly to `ICommand`s. This is useful if you want to handle the entire drag lifecycle in your ViewModel.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| DragEnterCommand | `ICommand` | Gets or sets the command invoked on drag enter. |
| CanExecuteDragEnterCommand | `bool` | Read-only value that observes whether DragEnterCommand can execute without an event-specific parameter. |
| DragOverCommand | `ICommand` | Gets or sets the command invoked on drag over. |
| CanExecuteDragOverCommand | `bool` | Read-only value that observes whether DragOverCommand can execute without an event-specific parameter. |
| DragLeaveCommand | `ICommand` | Gets or sets the command invoked on drag leave. |
| CanExecuteDragLeaveCommand | `bool` | Read-only value that observes whether DragLeaveCommand can execute without an event-specific parameter. |
| DropCommand | `ICommand` | Gets or sets the command invoked on drop. |
| CanExecuteDropCommand | `bool` | Read-only value that observes whether DropCommand can execute without an event-specific parameter. |
| CanExecuteCommandParameter | `object` | Optional parameter used to evaluate all drag-and-drop commands before event-specific parameters are available. |
| PassEventArgsToCommand | `bool` | Specifies whether the event args should be passed to the command. Default is true. |

When `PassEventArgsToCommand` is `true` and `CanExecuteCommandParameter` is not set, the `CanExecute*Command` properties report `true` without calling `CanExecute(null)` because the real `DragEventArgs` parameter is not available yet.
Set `CanExecuteCommandParameter` when command availability should still be observed before a drag event exists.
When `PassEventArgsToCommand` is `true`, execution still checks and executes the command with the current `DragEventArgs`.

## Usage

```xml
<Border Background="LightGray"
        Height="100"
        Width="100"
        IsEnabled="{Binding #DropCommands.CanExecuteDropCommand}">
    <Interaction.Behaviors>
        <DragDropCommandsBehavior x:Name="DropCommands"
                                  CanExecuteCommandParameter="{Binding CurrentDropTarget}"
                                  DropCommand="{Binding DropCommand}" />
    </Interaction.Behaviors>
</Border>
```
