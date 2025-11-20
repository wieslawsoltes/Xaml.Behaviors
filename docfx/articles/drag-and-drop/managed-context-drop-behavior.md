# ManagedContextDropBehavior

`ManagedContextDropBehavior` is a drop target behavior that integrates with `ManagedDragDropService`. It mirrors the semantics of `ContextDropBehavior` but works entirely in-process, handling drops initiated by `ManagedContextDragBehavior`.

## Properties

*   **`Context`**: The context required for the drop to be valid.
*   **`Handler`**: The `IDropHandler` that validates and executes the drop operation.

## Usage

```xml
<Border Background="Green" Width="100" Height="100">
    <Interaction.Behaviors>
        <ManagedContextDropBehavior Context="{Binding}" Handler="{StaticResource MyDropHandler}" />
    </Interaction.Behaviors>
</Border>
```
