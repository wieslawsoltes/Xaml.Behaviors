# ClearAutoCompleteBoxSelectionAction

This action clears both the `SelectedItem` and the `Text` property of an `AutoCompleteBox`. It is useful for "Reset" buttons or clearing the selection after an item has been picked and processed.

### Properties

*   **AutoCompleteBox**: The target `AutoCompleteBox` to clear. If not set, it attempts to use the `sender` (the control triggering the action) if it is an `AutoCompleteBox`.

### Example 1: Clearing a specific AutoCompleteBox

```xml
<StackPanel>
    <AutoCompleteBox Name="SearchBox" ItemsSource="{Binding Items}" />
    
    <Button Content="Clear Search">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <ClearAutoCompleteBoxSelectionAction AutoCompleteBox="{Binding #SearchBox}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>
</StackPanel>
```

### Example 2: Clearing self (if attached to the box)

If you attach the behavior directly to the `AutoCompleteBox` (e.g., clearing on some event), you don't need to specify the target.

```xml
<AutoCompleteBox ItemsSource="{Binding Items}">
    <Interaction.Behaviors>
        <!-- Example: Clear when focus is lost -->
        <EventTriggerBehavior EventName="LostFocus">
            <ClearAutoCompleteBoxSelectionAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</AutoCompleteBox>
```
