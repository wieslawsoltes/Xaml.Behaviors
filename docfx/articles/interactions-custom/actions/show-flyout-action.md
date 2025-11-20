# ShowFlyoutAction

Shows the `Flyout` attached to the target control.

```xml
<Button Content="Has Flyout" Name="FlyoutButton">
    <Button.Flyout>
        <Flyout>
            <TextBlock Text="Flyout Content" />
        </Flyout>
    </Button.Flyout>
    
    <Interaction.Behaviors>
        <!-- Open flyout on DoubleTapped instead of Click -->
        <EventTriggerBehavior EventName="DoubleTapped">
            <ShowFlyoutAction TargetObject="{Binding #FlyoutButton}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
