# BindTagToVisualRootDataContextBehavior

This behavior binds the `Tag` property of the associated control to the `DataContext` of the visual root (usually the `Window` or `UserControl`). This is a convenient way to access the root ViewModel from deep within the visual tree without complex relative bindings.

### Example

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <BindTagToVisualRootDataContextBehavior />
    </Interaction.Behaviors>
    <!-- Now Tag holds the Root ViewModel -->
    <Button.CommandParameter>
        <Binding Path="Tag.SomeGlobalCommand" RelativeSource="{RelativeSource Self}" />
    </Button.CommandParameter>
</Button>
```
