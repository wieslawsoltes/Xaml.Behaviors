# ScrollToOffsetAction

Scrolls a `ScrollViewer` to the specified offsets when executed.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| ScrollViewer | `ScrollViewer` | Gets or sets the `ScrollViewer` instance to scroll. If not set, it attempts to use the sender. |
| HorizontalOffset | `double?` | Gets or sets the horizontal offset to scroll to. |
| VerticalOffset | `double?` | Gets or sets the vertical offset to scroll to. |

## Usage

```xml
<Button Content="Scroll To Top">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ScrollToOffsetAction ScrollViewer="{Binding #MyScrollViewer}" VerticalOffset="0" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<ScrollViewer x:Name="MyScrollViewer">
    <!-- Content -->
</ScrollViewer>
```
