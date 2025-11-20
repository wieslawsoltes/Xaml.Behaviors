# SelectingItemsControlSearchBehavior

Filters `SelectingItemsControl` items based on the text of a search box.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| SearchBox | `TextBox` | Gets or sets the search box control. |
| NoMatchesControl | `Control` | Gets or sets the control displayed when no matches are found. |
| EnableSorting | `bool` | Gets or sets a value indicating whether items should be sorted. |
| SortOrder | `SortDirection` | Gets or sets the sort order for the items. Options are `Ascending` or `Descending`. |

## Usage

```xml
<StackPanel>
    <TextBox x:Name="SearchBox" Watermark="Search..." />
    <TextBlock x:Name="NoMatches" Text="No matches found" IsVisible="False" />
    
    <ListBox ItemsSource="{Binding Items}">
        <Interaction.Behaviors>
            <SelectingItemsControlSearchBehavior SearchBox="{Binding #SearchBox}"
                                                 NoMatchesControl="{Binding #NoMatches}"
                                                 EnableSorting="True"
                                                 SortOrder="Ascending" />
        </Interaction.Behaviors>
    </ListBox>
</StackPanel>
```
