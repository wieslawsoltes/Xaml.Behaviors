# InlineEditBehavior

This behavior facilitates an "inline edit" pattern where a control switches between a display mode (e.g., `TextBlock`) and an edit mode (e.g., `TextBox`).

### Properties
*   `DisplayControl`: The control visible in display mode.
*   `EditControl`: The control visible in edit mode.
*   `EditKey`: The key to switch to edit mode (default: F2).
*   `AcceptKey`: The key to commit changes and switch back (default: Enter).
*   `CancelKey`: The key to cancel changes and switch back (default: Escape).
*   `EditOnAssociatedObjectDoubleTapped`: If true, double-tapping the container triggers edit mode.

### Example

```xml
<StackPanel Orientation="Horizontal">
    <Interaction.Behaviors>
        <InlineEditBehavior DisplayControl="{Binding #NameBlock}"
                            EditControl="{Binding #NameBox}"
                            EditOnAssociatedObjectDoubleTapped="True" />
    </Interaction.Behaviors>

    <TextBlock Name="NameBlock" Text="{Binding Name}" VerticalAlignment="Center" />
    <TextBox Name="NameBox" Text="{Binding Name}" IsVisible="False" />
</StackPanel>
```
