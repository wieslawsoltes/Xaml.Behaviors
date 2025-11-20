# ContextDialogBehavior

This behavior allows you to define a modal-like or context-aware dialog directly in XAML, attached to a specific control. It uses a `Popup` internally to display content.

### Properties

*   `DialogContent`: The content to display inside the dialog. This can be any `Control`.
*   `IsOpen`: A boolean property to control the visibility of the dialog.
*   `Placement`: Determines where the dialog appears relative to the attached control (e.g., `Bottom`, `Right`, `Center`).
*   `IsLightDismissEnabled`: If `true` (default), clicking outside the dialog will close it.

### Events
*   `Opened`: Raised when the dialog opens.
*   `Closed`: Raised when the dialog closes.

### Example

```xml
<Button Content="Open Dialog">
    <Interaction.Behaviors>
        <ContextDialogBehavior x:Name="MyDialog" Placement="Bottom">
            <ContextDialogBehavior.DialogContent>
                <Border Background="White" BorderBrush="Gray" BorderThickness="1" Padding="10">
                    <StackPanel Spacing="10">
                        <TextBlock Text="This is a context dialog!" />
                        <Button Content="Close Me">
                            <Interaction.Behaviors>
                                <EventTriggerBehavior EventName="Click">
                                    <HideContextDialogAction TargetDialog="{Binding #MyDialog}" />
                                </EventTriggerBehavior>
                            </Interaction.Behaviors>
                        </Button>
                    </StackPanel>
                </Border>
            </ContextDialogBehavior.DialogContent>
        </ContextDialogBehavior>
        
        <!-- Open the dialog when the button is clicked -->
        <EventTriggerBehavior EventName="Click">
            <ShowContextDialogAction TargetDialog="{Binding #MyDialog}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
