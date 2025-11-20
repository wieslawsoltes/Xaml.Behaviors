# ShowContextMenuAction

Forces the Context Menu of a control to open.

```xml
<Border Background="Transparent" Height="100" Width="100">
    <Border.ContextMenu>
        <ContextMenu>
            <MenuItem Header="Option 1" />
            <MenuItem Header="Option 2" />
        </ContextMenu>
    </Border.ContextMenu>

    <Interaction.Behaviors>
        <!-- Open ContextMenu on Left Click instead of Right Click -->
        <EventTriggerBehavior EventName="PointerPressed">
            <ShowContextMenuAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Border>
```
