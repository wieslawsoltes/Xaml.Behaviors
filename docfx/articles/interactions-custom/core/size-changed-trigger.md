# SizeChangedTrigger

Executes actions when the `SizeChanged` event is raised on the associated object.

### Usage Example

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <RoutedEventTrigger RoutedEvent="{x:Static Button.ClickEvent}">
            <InvokeCommandAction Command="{Binding ClickCommand}" />
        </RoutedEventTrigger>
    </Interaction.Behaviors>
</Button>
```
