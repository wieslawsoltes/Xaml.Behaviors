# SplitViewPaneOpeningTrigger

Executes actions when the `SplitView.PaneOpening` event is raised.

## Usage

```xml
<SplitView>
    <Interaction.Behaviors>
        <SplitViewPaneOpeningTrigger>
            <InvokeCommandAction Command="{Binding PaneOpeningCommand}" />
        </SplitViewPaneOpeningTrigger>
    </Interaction.Behaviors>
    <!-- Content -->
</SplitView>
```
