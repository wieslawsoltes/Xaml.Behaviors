# TreeViewFilterBehavior

Filters `TreeView` items based on the text of a search box.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| SearchBox | `TextBox` | Gets or sets the search box control. |
| NoMatchesControl | `Control` | Gets or sets the control displayed when no matches are found. |

## Usage

```xml
<StackPanel>
    <TextBox x:Name="SearchBox" Watermark="Search..." />
    <TextBlock x:Name="NoMatches" Text="No matches found" IsVisible="False" />
    
    <TreeView>
        <Interaction.Behaviors>
            <TreeViewFilterBehavior SearchBox="{Binding #SearchBox}" NoMatchesControl="{Binding #NoMatches}" />
        </Interaction.Behaviors>
        <!-- Items -->
    </TreeView>
</StackPanel>
```
