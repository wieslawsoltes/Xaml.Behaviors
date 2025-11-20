# ScrollViewerOffsetBehavior

Sets the `ScrollViewer.Offset` of the associated `ScrollViewer`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| HorizontalOffset | `double?` | Gets or sets the horizontal offset value. |
| VerticalOffset | `double?` | Gets or sets the vertical offset value. |

## Usage

```xml
<ScrollViewer>
    <Interaction.Behaviors>
        <ScrollViewerOffsetBehavior VerticalOffset="{Binding CurrentOffset}" />
    </Interaction.Behaviors>
    <!-- Content -->
</ScrollViewer>
```
