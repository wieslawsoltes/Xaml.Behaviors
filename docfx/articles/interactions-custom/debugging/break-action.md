# BreakAction

An action that triggers a debugger break.

## Usage

```xml
<Button Content="Debug Break">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <BreakAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
