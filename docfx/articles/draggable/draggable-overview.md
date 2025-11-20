# Draggable Overview

The `Xaml.Behaviors.Interactions.Draggable` package provides behaviors that enable dragging controls and items within containers. This includes dragging elements on a Canvas, reordering items in a ListBox, or simply moving a control around with the mouse.

## Namespaces

The package contains the following namespace:

*   **`Avalonia.Xaml.Interactions.Draggable`**: Contains all the draggable behaviors.

## Installation

```bash
dotnet add package Xaml.Behaviors.Interactions.Draggable
```

## Key Behaviors

*   **`CanvasDragBehavior`**: Allows dragging child controls within a `Canvas`.
*   **`MouseDragElementBehavior`**: Allows dragging a control using `TranslateTransform`.
*   **`ListReorderDragBehavior`**: Enables reordering items in an `ItemsControl` via drag-and-drop.
*   **`GridDragBehavior`**: Allows dragging child controls within a `Grid` (swapping rows/columns).
