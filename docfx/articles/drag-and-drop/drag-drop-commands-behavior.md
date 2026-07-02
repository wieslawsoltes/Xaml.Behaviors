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
| PassEventArgsToCommand | `bool` | Specifies whether the event args should be passed to the command. Default is true. |

The `CanExecute*Command` properties are evaluated before a specific drag event is available, so they call `CanExecute(null)`.
If a command's `CanExecute` requires `DragEventArgs`, keep the control enabled independently or make `CanExecute` tolerate `null`.
When `PassEventArgsToCommand` is `true`, execution still checks and executes the command with the current `DragEventArgs`.

## Usage

```xml
<Border Background="LightGray"
        Height="100"
        Width="100"
        IsEnabled="{Binding #DropCommands.CanExecuteDropCommand}">
    <Interaction.Behaviors>
        <DragDropCommandsBehavior x:Name="DropCommands"
                                  PassEventArgsToCommand="False"
                                  DropCommand="{Binding DropCommand}" />
    </Interaction.Behaviors>
</Border>
```
