# ListReorderDragBehavior

`ListReorderDragBehavior` enables users to reorder items within an `ItemsControl` (like a `ListBox`) by dragging them.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| PlaceholderTemplate | `ITemplate` | Gets or sets template used to build placeholder shown while reordering. |

*   **`PlaceholderTemplate`**: An optional `DataTemplate` to display a visual placeholder at the insertion point while dragging.

## Usage

Attach the behavior to individual items of the `ItemsControl`.

```xml
<ListBox ItemsSource="{Binding Items}">
    <ListBox.Styles>
        <Style Selector="ListBoxItem">
            <Setter Property="(Interaction.Behaviors)">
                <BehaviorCollectionTemplate>
                    <BehaviorCollection>
                        <ListReorderDragBehavior Orientation="Vertical" />
                    </BehaviorCollection>
                </BehaviorCollectionTemplate>
            </Setter>
        </Style>
    </ListBox.Styles>
    
    <ListBox.ItemTemplate>
        <DataTemplate>
            <Border Background="White" BorderBrush="Gray" BorderThickness="1" Padding="10">
                <TextBlock Text="{Binding Name}" />
            </Border>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

## ItemDragBehavior

`ItemDragBehavior` is the base class for `ListReorderDragBehavior`. It provides the fundamental logic for tracking drag operations within an `ItemsControl` but does not implement the reordering logic itself. Use `ListReorderDragBehavior` for standard reordering scenarios.
