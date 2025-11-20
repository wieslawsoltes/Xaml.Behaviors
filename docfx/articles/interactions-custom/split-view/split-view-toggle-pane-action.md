# SplitViewTogglePaneAction

Toggles the `SplitView.IsPaneOpen` state of a `SplitView` when executed.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TargetSplitView | `SplitView` | Gets or sets the target `SplitView`. If not set, the sender is used. |

## Usage

```xml
<Button Content="Toggle Pane">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <SplitViewTogglePaneAction TargetSplitView="{Binding #MySplitView}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>

<SplitView x:Name="MySplitView">
    <!-- Content -->
</SplitView>
```
