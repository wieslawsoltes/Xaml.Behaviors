# ScrollToControlAction

An action that brings a target control into view (scrolls to it).

## Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| TargetControl | Control | Gets or sets the control to scroll to. |

## Usage

```xml
<Button Content="Go to Bottom">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ScrollToControlAction TargetControl="{Binding ElementName=BottomControl}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
