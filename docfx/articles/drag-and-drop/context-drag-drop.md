# Context Drag & Drop

The `ContextDragBehavior` and `ContextDropBehavior` are the workhorses of this package. They allow you to easily implement drag-and-drop functionality where the data being moved is the `DataContext` of the source control.

## ContextDragBehavior

Attaching this behavior to a control makes it draggable. When the drag starts, the behavior captures the `DataContext` of the control.

### Properties

*   **`Handler`**: An implementation of `IDragHandler` that allows you to intercept and customize the drag start event.

### IDragHandler Interface

```csharp
public interface IDragHandler
{
    void BeforeDragDrop(object? sender, PointerEventArgs e, object? context);
    void AfterDragDrop(object? sender, PointerEventArgs e, object? context);
}
```

*   **`BeforeDragDrop`**: Called before the drag operation starts. You can use this to set up state or cancel the drag (though the behavior handles the actual initiation).
*   **`AfterDragDrop`**: Called after the drag operation completes (whether successful or not).

## ContextDropBehavior

Attaching this behavior to a control makes it a drop target.

### Properties

*   **`Handler`**: An implementation of `IDropHandler` that contains the logic for validating and executing the drop.

### IDropHandler Interface

```csharp
public interface IDropHandler
{
    void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);
    void Over(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);
    void Drop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);
    void Leave(object? sender, RoutedEventArgs e);
    bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state);
    bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state);
}
```

*   **`Enter` / `Over` / `Leave`**: Standard drag events.
*   **`Validate`**: Called to determine if the drop is valid. Return `true` to allow the drop (sets `DragEffects`).
*   **`Execute`**: Called when the drop actually happens. Perform your data manipulation here.
*   **`Drop`**: Called after `Execute`.

**Note**: The `DropHandlerBase` class provides a default implementation for `Enter`, `Over`, `Drop`, and `Leave` that calls `Validate` and `Execute`. You typically only need to override `Validate` and `Execute` when inheriting from `DropHandlerBase`.

## Example: Dragging Items Between Lists

### 1. Create the Drop Handler

```csharp
public class ListBoxDropHandler : DropHandlerBase
{
    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        // Allow dropping only if the source is an ItemViewModel and target is a MainViewModel (the list owner)
        return sourceContext is ItemViewModel && targetContext is MainViewModel;
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (sourceContext is ItemViewModel item && targetContext is MainViewModel vm)
        {
            // Move the item
            vm.Items.Add(item);
            return true;
        }
        return false;
    }
}
```

### 2. Setup XAML

```xml
<UserControl>
    
    <UserControl.Resources>
        <local:ListBoxDropHandler x:Key="ListBoxDropHandler" />
    </UserControl.Resources>

    <ListBox ItemsSource="{Binding Items}">
        <ListBox.Styles>
            <Style Selector="ListBoxItem">
                <Setter Property="(Interaction.Behaviors)">
                    <BehaviorCollection>
                        <!-- Make items draggable -->
                        <ContextDragBehavior />
                    </BehaviorCollection>
                </Setter>
            </Style>
        </ListBox.Styles>
        
        <Interaction.Behaviors>
            <!-- Make the ListBox a drop target -->
            <ContextDropBehavior Handler="{StaticResource ListBoxDropHandler}" />
        </Interaction.Behaviors>
    </ListBox>
</UserControl>
```
