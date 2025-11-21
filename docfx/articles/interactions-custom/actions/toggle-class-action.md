# ToggleClassAction

Toggles a specified `ClassName` in the `StyledElement.Classes` collection when invoked.

## Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| ClassName | string | Gets or sets the class name that should be toggled. |
| StyledElement | StyledElement | Gets or sets the target styled element that class name that should be toggled on. |

## Usage

```xml
<Button Content="Toggle Class">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ToggleClassAction ClassName=":active" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
