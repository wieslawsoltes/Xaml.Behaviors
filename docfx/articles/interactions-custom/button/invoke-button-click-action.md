# InvokeButtonClickAction

This action programmatically invokes a target `Button`. It follows Avalonia's normal button activation path, including the `Click` event, flyout handling, and command execution. Disabled buttons are not invoked.

### Properties
*   `TargetButton`: The button to click. If not set, it attempts to use the `sender` if it is a button.

### Example

```xml
<StackPanel>
    <Button Name="SubmitButton"
            Content="Submit"
            Command="{Binding SubmitCommand}" />

    <TextBox>
        <Interaction.Behaviors>
            <!-- Pressing Enter in the TextBox clicks the Submit button -->
            <EventTriggerBehavior EventName="KeyDown">
                <InvokeButtonClickAction TargetButton="{Binding #SubmitButton}" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </TextBox>
</StackPanel>
```

If a `Click` handler marks the event as handled, the button command is not executed, matching Avalonia's native button behavior.
