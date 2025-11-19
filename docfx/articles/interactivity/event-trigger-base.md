# EventTriggerBase

`EventTriggerBase` is a base class for triggers that listen for a specific event on a source object.

## Properties

*   **`EventName`**: The name of the event to listen for.
*   **`SourceObject`**: The object that raises the event. If not set, it defaults to the `AssociatedObject`.

## Functionality

`EventTriggerBase` handles the complexity of finding the event (using reflection or the [Event Contracts](event-contracts.md) system) and subscribing/unsubscribing to it. When the event is raised, it invokes the actions associated with the trigger.

## Usage

This is the most common base class for creating event-driven triggers.

```csharp
public class MyEventTrigger : EventTriggerBase<Button>
{
    // ...
}
```
