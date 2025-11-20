# DragDropCommandsBehavior

`DragDropCommandsBehavior` maps Drag and Drop events directly to `ICommand`s. This is useful if you want to handle the entire drag lifecycle in your ViewModel.

## Properties

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
