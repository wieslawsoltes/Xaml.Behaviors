# Drag & Drop Overview

The `Xaml.Behaviors.Interactions.DragAndDrop` package provides a powerful and flexible set of behaviors to handle Drag and Drop operations in Avalonia applications. It abstracts away much of the boilerplate code required for standard drag-and-drop scenarios, allowing you to focus on the business logic.

## Namespaces

The package contains the following namespace:

*   **`Avalonia.Xaml.Interactions.DragAndDrop`**: Contains all the behaviors, actions, and interfaces.

## Installation

```bash
dotnet add package Xaml.Behaviors.Interactions.DragAndDrop
```

## Key Concepts

### Context-Awareness
Most behaviors in this package are "context-aware". This means they operate on the `DataContext` of the control they are attached to. When you drag an item, the behavior automatically captures its `DataContext` as the payload. When you drop an item, the handler receives both the source payload and the target's `DataContext`.

### Handlers
The logic for what happens during a drag or drop operation is encapsulated in **Handlers**:

*   **`IDragHandler`**: Controls the start of a drag operation (e.g., setting the visual effect, validating the drag).
*   **`IDropHandler`**: Controls the drop operation (e.g., validating the drop target, executing the data transfer).

By separating the behavior (XAML) from the logic (C# Handler), you keep your ViewModels clean and your XAML declarative.

## Available Behaviors

*   **`ContextDragBehavior`** / **`ContextDropBehavior`**: The core behaviors for moving data between controls.
*   **`FilesDropBehavior`**: Specialized behavior for handling file drops from the operating system.
*   **`TypedDragBehavior`**: Initiates drag operations only for specific data types.
*   **`DragAndDropEventsBehavior`**: Exposes standard Drag and Drop events as commands or actions.
