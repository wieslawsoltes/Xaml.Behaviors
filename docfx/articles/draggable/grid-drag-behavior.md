# GridDragBehavior

`GridDragBehavior` allows dragging child controls within a `Grid`. When a control is dragged over another cell, it can swap positions (Row/Column) with the control in that cell.

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
