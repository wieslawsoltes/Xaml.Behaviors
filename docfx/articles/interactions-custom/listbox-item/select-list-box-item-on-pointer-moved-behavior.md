# SelectListBoxItemOnPointerMovedBehavior

Sets the `IsSelected` property of the associated `ListBoxItem` to `true` when the `PointerMoved` event occurs. This creates a "hover to select" effect.

### Usage Example

This behavior is typically applied within the `ItemContainerTheme` or `ItemTemplate` of a `ListBox`.

```xml
<ListBox>
    <ListBox.Styles>
        <Style Selector="ListBoxItem">
            <Setter Property="(Interaction.Behaviors)">
                <BehaviorCollection>
                    <SelectListBoxItemOnPointerMovedBehavior />
                </BehaviorCollection>
            </Setter>
        </Style>
    </ListBox.Styles>
    <!-- Items -->
</ListBox>
```
