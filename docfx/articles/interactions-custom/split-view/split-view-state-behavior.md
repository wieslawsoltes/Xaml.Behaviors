# SplitViewStateBehavior

Updates `SplitView` properties based on size conditions.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| SourceControl | `Control` | Gets or sets the control whose bounds are observed. If not set, the associated object is used. |
| Setters | `AvaloniaList<SplitViewStateSetter>` | Gets split view state setters collection. |

## Usage

```xml
<SplitView DisplayMode="Inline" IsPaneOpen="True">
    <Interaction.Behaviors>
        <SplitViewStateBehavior>
            <SplitViewStateSetter MinWidth="0" MaxWidth="500" DisplayMode="Overlay" IsPaneOpen="False" />
            <SplitViewStateSetter MinWidth="500" DisplayMode="Inline" IsPaneOpen="True" />
        </SplitViewStateBehavior>
    </Interaction.Behaviors>
    <!-- Content -->
</SplitView>
```
