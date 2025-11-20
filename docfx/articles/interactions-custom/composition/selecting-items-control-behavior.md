# SelectingItemsControlBehavior

The `SelectingItemsControlBehavior` provides a smooth selection animation for `SelectingItemsControl` (e.g., `ListBox`, `TabControl`) using composition animations. It animates a visual element (usually a border or indicator) to move from the previously selected item to the newly selected item.

### Usage

To use this behavior, you need to:
1.  Attach the `EnableSelectionAnimation` property to your `SelectingItemsControl`.
2.  Ensure your item container template contains a visual element named `PART_SelectedPipe`. This element will be animated.

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `EnableSelectionAnimation` | `bool` | Attached property. When set to `True`, enables the selection animation on the `SelectingItemsControl`. |

### Example

```xml
<ListBox Background="Transparent"
         SelectingItemsControlBehavior.EnableSelectionAnimation="True">
    <ListBox.Styles>
        <Style Selector="ListBoxItem">
            <Setter Property="Template">
                <ControlTemplate>
                    <Grid Background="Transparent">
                        <Border Name="PART_SelectedPipe"
                                Background="Blue"
                                Height="2"
                                VerticalAlignment="Bottom"
                                IsVisible="{TemplateBinding IsSelected}" />
                        <ContentPresenter Content="{TemplateBinding Content}"
                                          Margin="10" />
                    </Grid>
                </ControlTemplate>
            </Setter>
        </Style>
    </ListBox.Styles>
    <ListBoxItem>Item 1</ListBoxItem>
    <ListBoxItem>Item 2</ListBoxItem>
    <ListBoxItem>Item 3</ListBoxItem>
</ListBox>
```
