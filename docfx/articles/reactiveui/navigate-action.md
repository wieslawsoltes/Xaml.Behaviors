# NavigateAction

`NavigateAction` is an action that navigates to a specified `IRoutableViewModel` using ReactiveUI's routing system.

## Properties

*   **`Router`**: The `RoutingState` to use for navigation.
*   **`ViewModel`**: The `IRoutableViewModel` instance to navigate to.

## Usage

```xml
<Button Content="Navigate">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <NavigateAction Router="{Binding Router}" ViewModel="{Binding NextViewModel}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
