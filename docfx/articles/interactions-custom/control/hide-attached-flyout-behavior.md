# HideAttachedFlyoutBehavior

Automatically hides the `Flyout` attached to the control when a bound boolean property becomes `false`.

### Properties
*   `IsFlyoutOpen`: Bind this to a boolean property. Setting it to `false` closes the flyout.

### Example

```xml
<Button Content="Options">
    <Button.Flyout>
        <Flyout>
            <TextBlock Text="Menu" />
        </Flyout>
    </Button.Flyout>
    <Interaction.Behaviors>
        <HideAttachedFlyoutBehavior IsFlyoutOpen="{Binding IsMenuOpen}" />
    </Interaction.Behaviors>
</Button>
```
