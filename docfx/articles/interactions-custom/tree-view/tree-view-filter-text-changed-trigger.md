# TreeViewFilterTextChangedTrigger

Executes actions when the search box text changes.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| SearchBox | `TextBox` | Gets or sets the search box control. |

## Usage

```xml
<TextBox x:Name="SearchBox" Watermark="Search..." />
<TreeView x:Name="MyTreeView">
    <Interaction.Behaviors>
        <TreeViewFilterTextChangedTrigger SearchBox="{Binding #SearchBox}">
            <ApplyTreeViewFilterAction TreeView="{Binding #MyTreeView}" Query="{Binding #SearchBox.Text}" />
        </TreeViewFilterTextChangedTrigger>
    </Interaction.Behaviors>
    <!-- Items -->
</TreeView>
```
