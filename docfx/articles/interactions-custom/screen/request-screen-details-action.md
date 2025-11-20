# RequestScreenDetailsAction

`RequestScreenDetailsAction` is an action that requests extended screen information from `Screens`. This is typically used on platforms (like Web) where screen details might need explicit permission or an async request.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Screens | `Screens` | Gets or sets the `Screens` instance used to request screen details. If not set, the screens of the associated `TopLevel` will be used. |

## Usage

```xml
<Button Content="Request Screen Details">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <RequestScreenDetailsAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
