# AddItemToItemsControlAction

Adds a new item to the end of the collection.
- `Item`: The item to add.

## Uno Platform
WinUI templates only create UI elements (there is no `ObjectTemplate`), so the Uno Platform action has an `ItemFactory`
property: an `IItemFactory` whose `CreateItem()` creates a new item to add on every execution. When set, it is used
instead of `Item`.
