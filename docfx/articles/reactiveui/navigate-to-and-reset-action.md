# NavigateToAndResetAction

`NavigateToAndResetAction<TViewModel>` is an action that resolves a view model from the service locator, navigates to it, and resets the navigation stack.

## Properties

*   **`Router`**: The `RoutingState` to use for navigation.
*   **`TViewModel`**: The type of the view model to navigate to (specified via `x:TypeArguments`).

## Usage

```xml
<Button Content="Home">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <NavigateToAndResetAction x:TypeArguments="vm:HomeViewModel" Router="{Binding Router}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
