# ManagedContextDragBehavior

`ManagedContextDragBehavior` initiates an in-process managed drag operation with an optional preview window. This behavior avoids the operating system's drag-and-drop mechanism and integrates with `ManagedDragDropService`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Context | `object` | Gets or sets the context value used as a drag payload when the drag starts. |
| PreviewTemplate | `IDataTemplate` | Gets or sets the template used to render the drag preview. |
| HorizontalDragThreshold | `double` | Gets or sets the minimal horizontal distance required to start dragging. Default is 3. |
| VerticalDragThreshold | `double` | Gets or sets the minimal vertical distance required to start dragging. Default is 3. |
| PreviewOffset | `Point` | Gets or sets the fixed logical offset applied to the preview position. |
| DataFormat | `string` | Gets or sets the data format name used to identify the payload. Default is "Context". |
| UsePointerRelativePreviewOffset | `bool` | Gets or sets whether to compute a pointer-relative preview offset automatically. Default is true. |
| PreviewOpacity | `double` | Gets or sets the preview window opacity. Default is 0.65. |

## Usage

```xml
<Border Background="Red" Width="50" Height="50">
    <Interaction.Behaviors>
        <ManagedContextDragBehavior Context="{Binding}" />
    </Interaction.Behaviors>
</Border>
```

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
