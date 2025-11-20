# GridDragBehavior

`GridDragBehavior` allows dragging child controls within a `Grid`. When a control is dragged over another cell, it can swap positions (Row/Column) with the control in that cell.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| CopyColumn | `bool` | Gets or sets whether to copy the dragged element's column. Default is true. |
| CopyRow | `bool` | Gets or sets whether to copy the dragged element's row. Default is true. |
| CopyColumnSpan | `bool` | Gets or sets whether to copy the dragged element's column span. |
| CopyRowSpan | `bool` | Gets or sets whether to copy the dragged element's row span. |

## Usage

```xml
<Grid ColumnDefinitions="*,*" RowDefinitions="*,*">
    <Interaction.Behaviors>
        <GridDragBehavior />
    </Interaction.Behaviors>

    <Border Grid.Row="0" Grid.Column="0" Background="Red" />
    <Border Grid.Row="0" Grid.Column="1" Background="Blue" />
    <Border Grid.Row="1" Grid.Column="0" Background="Green" />
    <Border Grid.Row="1" Grid.Column="1" Background="Yellow" />
</Grid>
```
