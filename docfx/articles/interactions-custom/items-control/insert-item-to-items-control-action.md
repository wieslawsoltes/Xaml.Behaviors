# InsertItemToItemsControlAction

Inserts an item at a specific index.
- `Item`: The item to insert.
- `Index`: The insertion index.

## Uno Platform
WinUI templates only create UI elements (there is no `ObjectTemplate`), so the Uno Platform action has an `ItemFactory`
property: an `IItemFactory` whose `CreateItem()` creates a new item to insert on every execution. When set, it is used
instead of `Item`.
