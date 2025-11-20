# SplitViewPaneClosedTrigger

Executes actions when the `SplitView.PaneClosed` event is raised.

## Usage

```xml
<SplitView>
    <Interaction.Behaviors>
        <SplitViewPaneClosedTrigger>
            <InvokeCommandAction Command="{Binding PaneClosedCommand}" />
        </SplitViewPaneClosedTrigger>
    </Interaction.Behaviors>
    <!-- Content -->
</SplitView>
```
