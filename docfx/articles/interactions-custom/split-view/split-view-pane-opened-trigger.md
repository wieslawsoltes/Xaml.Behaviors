# SplitViewPaneOpenedTrigger

Executes actions when the `SplitView.PaneOpened` event is raised.

## Usage

```xml
<SplitView>
    <Interaction.Behaviors>
        <SplitViewPaneOpenedTrigger>
            <InvokeCommandAction Command="{Binding PaneOpenedCommand}" />
        </SplitViewPaneOpenedTrigger>
    </Interaction.Behaviors>
    <!-- Content -->
</SplitView>
```
