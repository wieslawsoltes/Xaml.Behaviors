# ListReorderDragBehavior

`ListReorderDragBehavior` enables users to reorder items within an `ItemsControl` (like a `ListBox`) by dragging them.

## Properties

*   **`PlaceholderTemplate`**: An optional `DataTemplate` to display a visual placeholder at the insertion point while dragging.

## Usage

Attach the behavior to the `ItemsControl`.

```xml
<ListBox ItemsSource="{Binding Items}">
    <Interaction.Behaviors>
        <ListReorderDragBehavior />
    </Interaction.Behaviors>
    
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
