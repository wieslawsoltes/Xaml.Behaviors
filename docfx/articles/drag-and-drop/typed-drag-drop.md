# Typed Drag & Drop

`TypedDragBehavior` is a variation of `ContextDragBehavior` that allows you to restrict drag operations to a specific data type.

## TypedDragBehavior

This behavior checks the `DataContext` of the associated object. If the `DataContext` matches the specified `DataType`, the drag operation is allowed to start.

### Properties

*   **`DataType`**: The `Type` of data that this behavior supports.
*   **`Handler`**: An optional `IDragHandler`.

## Usage

```xml
<UserControl xmlns:vm="using:MyApplication.ViewModels">

    <Border Background="LightGray" Padding="10">
        <Interaction.Behaviors>
            <TypedDragBehavior DataType="{x:Type vm:MyItemViewModel}" />
        </Interaction.Behaviors>
        <TextBlock Text="Drag me (if I am MyItemViewModel)" />
    </Border>

</UserControl>
```

This is particularly useful when you have a heterogeneous list of items and only want some of them to be draggable.
