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

## EventTrigger

`EventTrigger` is a deprecated class that functions similarly to `EventTriggerBehavior` but inherits from `Trigger`. It is recommended to use `EventTriggerBehavior` instead.
