# InvokeButtonClickAction

This action programmatically raises the `Click` event on a target `Button`. This is useful if you want to simulate a button click from another event or trigger.

### Properties
*   `TargetButton`: The button to click. If not set, it attempts to use the `sender` if it is a button.

### Example

```xml
<StackPanel>
    <Button Name="SubmitButton" Content="Submit" Click="SubmitButton_Click" />

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
