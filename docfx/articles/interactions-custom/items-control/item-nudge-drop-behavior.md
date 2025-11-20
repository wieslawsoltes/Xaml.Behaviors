# ItemNudgeDropBehavior

This behavior provides visual feedback during a drag-and-drop operation by "nudging" (translating) items in an `ItemsControl` to indicate where the dropped item would be inserted.

### Properties
- `Orientation`: Specifies the orientation of the `ItemsControl` (Horizontal or Vertical) to determine the direction of the nudge.

### Usage Example

```xml
<ListBox>
    <Interaction.Behaviors>
        <ItemNudgeDropBehavior Orientation="Vertical" />
    </Interaction.Behaviors>
</ListBox>
```
