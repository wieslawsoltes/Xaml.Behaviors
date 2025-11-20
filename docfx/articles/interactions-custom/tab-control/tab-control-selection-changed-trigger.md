# TabControlSelectionChangedTrigger

Executes actions when the associated `TabControl` changes selection.

## Usage

```xml
<TabControl>
    <Interaction.Behaviors>
        <TabControlSelectionChangedTrigger>
            <InvokeCommandAction Command="{Binding TabChangedCommand}" />
        </TabControlSelectionChangedTrigger>
    </Interaction.Behaviors>
    <!-- Tabs -->
</TabControl>
```
