# CapturePointerAction

Captures the pointer to the associated `InputElement`. This ensures that the control continues to receive pointer events even if the pointer moves outside its bounds.

### Usage Example

```xml
<Border Background="Red">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="PointerPressed">
            <CapturePointerAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Border>
```
