# SplitViewPaneClosingTrigger

Executes actions when the `SplitView.PaneClosing` event is raised.

## Usage

```xml
<SplitView>
    <Interaction.Behaviors>
        <SplitViewPaneClosingTrigger>
            <InvokeCommandAction Command="{Binding PaneClosingCommand}" />
        </SplitViewPaneClosingTrigger>
    </Interaction.Behaviors>
    <!-- Content -->
</SplitView>
```
