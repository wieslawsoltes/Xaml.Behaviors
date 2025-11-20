# ToggleIsExpandedOnDoubleTappedBehavior

Toggles `TreeViewItem.IsExpanded` property of the associated `TreeViewItem` control on `DoubleTapped` event.

## Usage

```xml
<TreeView>
    <TreeView.ItemTemplate>
        <TreeDataTemplate>
            <TextBlock Text="{Binding Name}">
                <Interaction.Behaviors>
                    <ToggleIsExpandedOnDoubleTappedBehavior />
                </Interaction.Behaviors>
            </TextBlock>
        </TreeDataTemplate>
    </TreeView.ItemTemplate>
</TreeView>
```
