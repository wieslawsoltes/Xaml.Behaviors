# EventTriggerBehavior

`EventTriggerBehavior` is a behavior that listens for a specific event on a source object and invokes actions when the event is raised.

## Properties

*   **`EventName`**: The name of the event to listen for.
*   **`SourceObject`**: The object that raises the event. If not set, it defaults to the `AssociatedObject`.

## Usage

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <InvokeCommandAction Command="{Binding MyCommand}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

### Flyout lifecycle events

Behaviors can be attached directly to a `Flyout` to observe its `Opened` and `Closed` events. When the flyout is shown, the behavior and its actions inherit the placement target's data context, so command bindings resolve normally.

```xml
<Button Content="Open menu">
    <Button.Flyout>
        <Flyout>
            <Interaction.Behaviors>
                <EventTriggerBehavior EventName="Opened">
                    <InvokeCommandAction Command="{Binding FlyoutOpenedCommand}" />
                </EventTriggerBehavior>
                <EventTriggerBehavior EventName="Closed">
                    <InvokeCommandAction Command="{Binding FlyoutClosedCommand}" />
                </EventTriggerBehavior>
            </Interaction.Behaviors>

            <TextBlock Text="Flyout content" />
        </Flyout>
    </Button.Flyout>
</Button>
```

## EventTrigger

`EventTrigger` is a deprecated class that functions similarly to `EventTriggerBehavior` but inherits from `Trigger`. It is recommended to use `EventTriggerBehavior` instead.
