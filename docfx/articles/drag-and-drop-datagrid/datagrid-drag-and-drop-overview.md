# DataGrid Drag & Drop Overview

The `Xaml.Behaviors.Interactions.DragAndDrop.DataGrid` package provides specialized support for Drag and Drop operations within an Avalonia `DataGrid`. It simplifies the implementation of row reordering and moving items between DataGrids.

## Namespaces

The package contains the following namespace:

*   **`Avalonia.Xaml.Interactions.DragAndDrop`**: Contains `BaseDataGridDropHandler<T>`.

## Installation

```bash
dotnet add package Xaml.Behaviors.Interactions.DragAndDrop.DataGrid
```

## Key Components

### BaseDataGridDropHandler\<T>

This abstract class handles the complexity of:
*   Determining the drop target row.
*   Calculating the drop position (above or below the row).
*   Applying visual feedback (Adorners) to indicate the insertion point.

To use it, you must create a class that inherits from `BaseDataGridDropHandler<T>` and implement the `Validate` and `MakeCopy` methods.

### Styles

The package includes default styles for the visual feedback (lines indicating where the row will be dropped). You must include these styles in your application (e.g., in `App.axaml` or your View).

```xml
<Application.Styles>
    <StyleInclude Source="avares://Xaml.Behaviors.Interactions.DragAndDrop.DataGrid/Styles.axaml" />
</Application.Styles>
```

## Usage Example

Here is how to implement a simple row reordering handler for a `DataGrid` displaying `ItemViewModel` objects.

### 1. Implement the Handler

```csharp
public class DataGridDropHandler : BaseDataGridDropHandler<ItemViewModel>
{
    protected override bool Validate(DataGrid dg, DragEventArgs e, object? sourceContext, object? targetContext, bool execute)
    {
        // Validate that we are dragging an ItemViewModel and dropping onto an ObservableCollection
        if (sourceContext is ItemViewModel sourceItem && 
            dg.ItemsSource is ObservableCollection<ItemViewModel> items)
        {
            // If we are just validating (execute=false), return true to indicate drop is allowed
            if (!execute) return true;

            // If executing, perform the move
            // targetContext is the item we are dropping onto (or null if empty/not on a row)
            var targetItem = targetContext as ItemViewModel;
            
            // Helper method from BaseDataGridDropHandler to handle Move/Copy logic
            // It calculates indices and moves the item in the collection
            return RunDropAction(dg, e, execute, sourceItem, targetItem, items);
        }
        return false;
    }

    protected override ItemViewModel MakeCopy(ObservableCollection<ItemViewModel> parentCollection, ItemViewModel item)
    {
        // Return a clone of the item if you support Copy operations
        return new ItemViewModel { Name = item.Name + " (Copy)" };
    }
}
```

### 2. Setup XAML

Apply the `ContextDragBehavior` to the rows (via `RowTheme`) and `ContextDropBehavior` to the `DataGrid`.

```xml
<UserControl>
    
    <UserControl.Resources>
        <local:DataGridDropHandler x:Key="DataGridDropHandler" />
    </UserControl.Resources>

    <DataGrid ItemsSource="{Binding Items}"
              Classes="DragAndDrop"> <!-- Important: Add this class for the styles to work -->
        
        <DataGrid.Styles>
            <Style Selector="DataGridRow">
                <Setter Property="(Interaction.Behaviors)">
                    <BehaviorCollection>
                        <!-- Make rows draggable -->
                        <ContextDragBehavior />
                    </BehaviorCollection>
                </Setter>
            </Style>
        </DataGrid.Styles>

        <Interaction.Behaviors>
            <!-- Make DataGrid a drop target -->
            <ContextDropBehavior Handler="{StaticResource DataGridDropHandler}" />
        </Interaction.Behaviors>

        <DataGrid.Columns>
            <DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        </DataGrid.Columns>
    </DataGrid>
</UserControl>
```

**Note**: The `Classes="DragAndDrop"` on the `DataGrid` is required for the default styles in `Styles.axaml` to apply the adorner feedback (the horizontal lines).
