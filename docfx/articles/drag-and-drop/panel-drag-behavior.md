# PanelDragBehavior

`PanelDragBehavior` starts a drag operation for a control so it can be moved between panels. The control itself is used as the drag context.

## Usage

```xml
<Canvas>
    <Rectangle Fill="Red" Width="50" Height="50" Canvas.Left="10" Canvas.Top="10">
        <Interaction.Behaviors>
            <PanelDragBehavior />
        </Interaction.Behaviors>
    </Rectangle>
</Canvas>
```

```xml
<Rectangle Fill="Blue" Width="50" Height="50">
    <Interaction.Behaviors>
        <PanelDragBehavior />
    </Interaction.Behaviors>
</Rectangle>
```
