# SelectingItemsControlEventsBehavior

An abstract base class for behaviors that listen to the `SelectingItemsControl.SelectionChanged` event.

## Remarks

This behavior is intended to be subclassed. It provides a `OnSelectionChanged` virtual method that is called when the selection changes.

## Methods

| Method | Description |
| --- | --- |
| `OnSelectionChanged(object? sender, SelectionChangedEventArgs e)` | Called when the selection changes. |
