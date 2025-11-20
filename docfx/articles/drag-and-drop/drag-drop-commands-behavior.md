# DragDropCommandsBehavior

`DragDropCommandsBehavior` maps Drag and Drop events directly to `ICommand`s. This is useful if you want to handle the entire drag lifecycle in your ViewModel.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| DragEnterCommand | `ICommand` | Gets or sets the command invoked on drag enter. |
| DragOverCommand | `ICommand` | Gets or sets the command invoked on drag over. |
| DragLeaveCommand | `ICommand` | Gets or sets the command invoked on drag leave. |
| DropCommand | `ICommand` | Gets or sets the command invoked on drop. |
| PassEventArgsToCommand | `bool` | Specifies whether the event args should be passed to the command. Default is true. |

## Usage

```xml
<Border Background="LightGray" Height="100" Width="100">
    <Interaction.Behaviors>
        <DragDropCommandsBehavior DropCommand="{Binding DropCommand}" />
    </Interaction.Behaviors>
</Border>
```

*   **`EnterCommand`**
*   **`LeaveCommand`**
*   **`OverCommand`**
*   **`DropCommand`**

## Usage

```xml
<Border>
    <Interaction.Behaviors>
        <DragDropCommandsBehavior EnterCommand="{Binding EnterCommand}"
                                  DropCommand="{Binding DropCommand}" />
    </Interaction.Behaviors>
</Border>
```
