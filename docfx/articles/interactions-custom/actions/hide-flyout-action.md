# HideFlyoutAction

Hides the `Flyout` attached to the target control.

```xml
<Button Content="Close Flyout">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <HideFlyoutAction TargetObject="{Binding #FlyoutButton}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
