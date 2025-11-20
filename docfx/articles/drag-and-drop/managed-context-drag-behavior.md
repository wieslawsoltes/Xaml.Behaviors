# ManagedContextDragBehavior

`ManagedContextDragBehavior` initiates an in-process managed drag operation with an optional preview window. This behavior avoids the operating system's drag-and-drop mechanism and integrates with `ManagedDragDropService`.

## Properties

*   **`Context`**: The context value used as the drag payload when the drag starts.
*   **`PreviewTemplate`**: The `IDataTemplate` used to render the drag preview window content.
*   **`HorizontalDragThreshold`**: The minimal horizontal distance required to start dragging (default is 3).

## Usage

```xml
<Border Background="Red" Width="50" Height="50">
    <Interaction.Behaviors>
        <ManagedContextDragBehavior Context="{Binding}" />
    </Interaction.Behaviors>
</Border>
```
