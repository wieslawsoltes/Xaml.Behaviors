# FluidMoveBehavior

The `FluidMoveBehavior` animates the position of an element or its children when the layout changes. This is useful for creating smooth transitions when items are added, removed, or reordered in a container.

### Properties

- `AppliesTo`: Determines whether the behavior applies to the element itself (`Self`) or its children (`Children`).
- `Duration`: The duration of the move animation.
- `Ease`: The easing function to use for the animation.
- `Tag`: An optional tag to filter which elements are animated (when using `Children` scope).

### Usage Example

```xml
<WrapPanel>
    <Interaction.Behaviors>
        <FluidMoveBehavior AppliesTo="Children" Duration="0:0:0.3" />
    </Interaction.Behaviors>
    <Button Content="Item 1" />
    <Button Content="Item 2" />
    <Button Content="Item 3" />
</WrapPanel>
```
