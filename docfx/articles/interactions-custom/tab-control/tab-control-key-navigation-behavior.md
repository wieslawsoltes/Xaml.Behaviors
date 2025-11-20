# TabControlKeyNavigationBehavior

Enables keyboard navigation for a `TabControl` using arrow keys.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Orientation | `Orientation` | Gets or sets the orientation used for navigation. |

## Usage

```xml
<TabControl>
    <Interaction.Behaviors>
        <TabControlKeyNavigationBehavior Orientation="Horizontal" />
    </Interaction.Behaviors>
    <TabItem Header="Tab 1">
        <TextBlock Text="Content 1" />
    </TabItem>
    <TabItem Header="Tab 2">
        <TextBlock Text="Content 2" />
    </TabItem>
</TabControl>
```
