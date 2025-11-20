# HorizontalScrollViewerBehavior

Enables horizontal scrolling of a `ScrollViewer` using the mouse wheel.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| RequireShiftKey | `bool` | Gets or sets a value indicating whether the Shift key must be held while scrolling. |
| ScrollChangeSize | `ChangeSize` | Gets or sets the scroll amount used for each wheel delta. Options are `Line` or `Page`. |

## Usage

```xml
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Disabled">
    <Interaction.Behaviors>
        <HorizontalScrollViewerBehavior RequireShiftKey="False" ScrollChangeSize="Line" />
    </Interaction.Behaviors>
    <StackPanel Orientation="Horizontal">
        <!-- Items -->
    </StackPanel>
</ScrollViewer>
```
