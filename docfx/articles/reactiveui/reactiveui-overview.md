# ReactiveUI Overview

The `Xaml.Behaviors.Interactions.ReactiveUI` package provides behaviors and actions that integrate with the [ReactiveUI](https://www.reactiveui.net/) MVVM framework.

## Namespaces

The package contains the following namespace:

*   **`Avalonia.Xaml.Interactions.ReactiveUI`**: Contains `InteractionTriggerBehavior` and navigation actions.

## Installation

```bash
dotnet add package Xaml.Behaviors.Interactions.ReactiveUI
```

## Key Components

*   **`InteractionTriggerBehavior`**: A trigger that listens to ReactiveUI's `Interaction<TInput, TOutput>` and executes actions when the interaction is requested.
*   **Navigation Actions**: A set of actions for performing navigation using ReactiveUI's `RoutingState`.
    *   `NavigateToAction<TViewModel>`
    *   `NavigateBackAction`
    *   `NavigateAndResetAction`
    *   `ClearNavigationStackAction`

These components allow you to handle view-related logic (like showing dialogs or navigating) directly from your ViewModels in a decoupled way.
