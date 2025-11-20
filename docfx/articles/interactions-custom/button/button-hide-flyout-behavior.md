# ButtonHideFlyoutBehavior

This behavior allows you to programmatically close a button's flyout by binding to a boolean property. When the property becomes `false`, the flyout is hidden.

### Properties
*   `IsFlyoutOpen`: Bind this to a boolean property. Setting it to `false` closes the flyout.

### Example

```xml
<Button Content="Open Flyout">
    <Button.Flyout>
        <Flyout>
            <TextBlock Text="Hello" />
        </Flyout>
    </Button.Flyout>
    <Interaction.Behaviors>
        <ButtonHideFlyoutBehavior IsFlyoutOpen="{Binding IsMenuOpen}" />
    </Interaction.Behaviors>
</Button>
```
