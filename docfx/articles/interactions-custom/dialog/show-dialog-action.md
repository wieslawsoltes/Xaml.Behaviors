# ShowDialogAction

Shows a specified `Window` as a modal dialog.

### Properties
- `Dialog`: The `Window` instance to show as a dialog.
- `Owner`: The owner `Window` for the dialog. If not specified, the action attempts to find the visual root of the sender or associated object.

### Usage Example

```xml
<Button Content="Show Dialog">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ShowDialogAction Dialog="{Binding ElementName=MyDialogWindow}" 
                              Owner="{Binding ElementName=MainWindow}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
