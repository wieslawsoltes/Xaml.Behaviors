# LaunchUriAction

An action that launches a URI using the system's default handler.

## Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| Uri | string | Gets or sets the URI to launch. |

## Usage

```xml
<Button Content="Open Website">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <LaunchUriAction Uri="https://avaloniaui.net" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
