# ManagedContextDropBehavior

`ManagedContextDropBehavior` is a drop target behavior that integrates with `ManagedDragDropService`. It mirrors the semantics of `ContextDropBehavior` but works entirely in-process, handling drops initiated by `ManagedContextDragBehavior`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| AcceptDataFormat | `string` | Gets or sets the accepted managed data format. Default is "Context". |
| AllowDrop | `bool` | Gets or sets a value indicating whether drop is allowed. Default is true. |
| OverClass | `string` | Gets or sets the pseudo-class to apply when a valid drag is over the control. |
| Context | `object` | Gets or sets the target context. |
| Handler | `IDropHandler` | Gets or sets the drop handler. |

## Usage

```xml
<Border Background="LightGray" Width="200" Height="200">
    <Interaction.Behaviors>
        <ManagedContextDropBehavior Handler="{Binding DropHandler}" />
    </Interaction.Behaviors>
</Border>
```

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
