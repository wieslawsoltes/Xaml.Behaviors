# ButtonHideFlyoutOnClickBehavior

When you have a button inside a `Flyout`, you often want the flyout to close immediately after the button is clicked. This behavior handles that logic automatically. It also ensures the button's `Command` is executed before the flyout closes.

### Example

```xml
<Button Content="Options">
    <Button.Flyout>
        <Flyout>
            <StackPanel>
                <Button Content="Save & Close" Command="{Binding SaveCommand}">
                    <Interaction.Behaviors>
                        <ButtonHideFlyoutOnClickBehavior />
                    </Interaction.Behaviors>
                </Button>
            </StackPanel>
        </Flyout>
    </Button.Flyout>
</Button>
```
