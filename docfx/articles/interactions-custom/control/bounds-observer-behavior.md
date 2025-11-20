# BoundsObserverBehavior

This behavior observes the `Bounds` of the associated control and exposes `Width` and `Height` properties that update whenever the bounds change. This allows you to bind other controls' dimensions to the actual rendered size of this control.

### Properties
*   `Bounds`: The current bounds (read-only).
*   `Width`: The current width.
*   `Height`: The current height.

### Example

```xml
<Panel>
    <Border Name="SourceBorder" Background="Red" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
        <Interaction.Behaviors>
            <BoundsObserverBehavior Width="{Binding BorderWidth, Mode=OneWayToSource}" 
                                        Height="{Binding BorderHeight, Mode=OneWayToSource}" />
        </Interaction.Behaviors>
    </Border>
    
    <TextBlock Text="{Binding BorderWidth, StringFormat='Width: {0}'}" />
</Panel>
```
