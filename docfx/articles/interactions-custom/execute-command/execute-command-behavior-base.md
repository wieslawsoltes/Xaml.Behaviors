# ExecuteCommandBehaviorBase

Base class for behaviors that execute a command.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TopLevel | TopLevel | The TopLevel to focus when FocusTopLevel is true. |
| Command | ICommand | The command to execute. |
| CommandParameter | object | The command parameter. |
| FocusTopLevel | bool | If true, the TopLevel will be focused after the command is executed. |
| FocusControl | Control | The control to focus after the command is executed. |
| SourceControl | Control | The control to use as the source for the behavior. If not set, the AssociatedObject is used. |

