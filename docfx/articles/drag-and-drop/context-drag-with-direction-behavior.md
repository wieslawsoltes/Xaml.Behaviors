# ContextDragWithDirectionBehavior

Behavior that starts drag and drop with information about drag direction.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Context | `object` | Gets or sets the context used for drag operations. |
| Handler | `IDragHandler` | Gets or sets the drag handler to notify. |
| HorizontalDragThreshold | `double` | Gets or sets the horizontal drag threshold. Default is 3. |
| VerticalDragThreshold | `double` | Gets or sets the vertical drag threshold. Default is 3. |

## Usage

```xml
<Border Background="Red" Width="100" Height="100">
    <Interaction.Behaviors>
        <ContextDragWithDirectionBehavior HorizontalDragThreshold="10" VerticalDragThreshold="10" />
    </Interaction.Behaviors>
</Border>
```
