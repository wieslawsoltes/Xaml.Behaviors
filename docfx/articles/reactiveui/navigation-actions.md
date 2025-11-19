# Navigation Actions

The `Xaml.Behaviors.Interactions.ReactiveUI` package provides several actions to integrate with ReactiveUI's routing system (`IScreen`, `RoutingState`).

## Available Actions

### NavigateToAction\<TViewModel>

Navigates to a new view model. It resolves the view model using `Locator.Current.GetService<TViewModel>()`.

*   **`Router`**: The `RoutingState` to use for navigation.
*   **`TViewModel`**: The type of the view model to navigate to (specified via `x:TypeArguments`).

### NavigateBackAction

Navigates back to the previous view model in the stack.

*   **`Router`**: The `RoutingState` to use for navigation.

### NavigateAndResetAction\<TViewModel>

Clears the navigation stack and navigates to the specified view model.

*   **`Router`**: The `RoutingState` to use for navigation.
*   **`TViewModel`**: The type of the view model to navigate to.

### ClearNavigationStackAction

Clears the navigation stack.

*   **`Router`**: The `RoutingState` to use for navigation.

## Usage

These actions are typically used with `EventTriggerBehavior` or `InteractionTriggerBehavior`.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:Sample.ViewModels"
             x:Class="Sample.Views.MainView">

    <Button Content="Go to Details">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <NavigateToAction x:TypeArguments="vm:DetailsViewModel" 
                                  Router="{Binding Router}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>

    <Button Content="Go Back">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <NavigateBackAction Router="{Binding Router}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</UserControl>
```

### Dependency Injection

The `NavigateToAction` and `NavigateAndResetAction` rely on `Splat.Locator` to resolve the view model instances. Ensure your view models are registered in the container before using these actions.

```csharp
Locator.CurrentMutable.Register(() => new DetailsViewModel(), typeof(DetailsViewModel));
```
