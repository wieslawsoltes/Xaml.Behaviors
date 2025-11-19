# Responsive Overview

The `Xaml.Behaviors.Interactions.Responsive` package provides behaviors that enable responsive design in Avalonia applications. These behaviors allow you to automatically apply or remove style classes (or pseudo-classes) based on the size or aspect ratio of a control.

## Namespaces

The package contains the following namespace:

*   **`Avalonia.Xaml.Interactions.Responsive`**: Contains `AdaptiveBehavior` and `AspectRatioBehavior`.

## Installation

```bash
dotnet add package Xaml.Behaviors.Interactions.Responsive
```

## Key Components

*   **`AdaptiveBehavior`**: Observes the bounds of a control and applies classes based on width and height conditions.
*   **`AspectRatioBehavior`**: Observes the bounds of a control and applies classes based on the aspect ratio (width / height).

These behaviors are particularly useful for creating adaptive layouts that respond to changes in window size or container dimensions without writing code-behind or complex converters.
