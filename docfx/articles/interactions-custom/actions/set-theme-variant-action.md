# SetThemeVariantAction

Changes the `ThemeVariant` (Light, Dark, Default) of a control.

```xml
<Button Content="Switch to Dark Mode">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <SetThemeVariantAction ThemeVariant="Dark" TargetObject="{Binding $parent[Window]}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
