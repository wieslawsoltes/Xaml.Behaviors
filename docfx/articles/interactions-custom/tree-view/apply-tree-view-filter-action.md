# ApplyTreeViewFilterAction

Filters a `TreeView` using the provided query string.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TreeView | `TreeView` | Gets or sets the tree view to filter. If not set, the sender is used. |
| Query | `string` | Gets or sets the filter query string. |

## Usage

```xml
<TextBox x:Name="SearchBox" Watermark="Search..." />
<Button Content="Apply Filter">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ApplyTreeViewFilterAction TreeView="{Binding #MyTreeView}" Query="{Binding #SearchBox.Text}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<TreeView x:Name="MyTreeView">
    <!-- Items -->
</TreeView>
```
