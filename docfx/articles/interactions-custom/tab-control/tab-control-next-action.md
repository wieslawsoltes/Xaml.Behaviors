# TabControlNextAction

Advances the target `TabControl` to the next tab.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TabControl | `TabControl` | Gets or sets the tab control instance this action will operate on. If not set, the sender is used. |

## Usage

```xml
<Button Content="Next Tab">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <TabControlNextAction TabControl="{Binding #MyTabControl}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<TabControl x:Name="MyTabControl">
    <!-- Tabs -->
</TabControl>
```
