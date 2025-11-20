# TabControlPreviousAction

Moves the target `TabControl` to the previous tab.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TabControl | `TabControl` | Gets or sets the tab control instance this action will operate on. If not set, the sender is used. |

## Usage

```xml
<Button Content="Previous Tab">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <TabControlPreviousAction TabControl="{Binding #MyTabControl}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<TabControl x:Name="MyTabControl">
    <!-- Tabs -->
</TabControl>
```
