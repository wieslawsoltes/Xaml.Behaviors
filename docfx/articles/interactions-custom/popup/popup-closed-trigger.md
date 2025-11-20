# PopupClosedTrigger

Executes actions when the associated `Popup` raises the `Closed` event.

### Usage Example

```xml
<Popup>
    <Interaction.Behaviors>
        <PopupClosedTrigger>
            <InvokeCommandAction Command="{Binding PopupClosedCommand}" />
        </PopupClosedTrigger>
    </Interaction.Behaviors>
    <!-- Content -->
</Popup>
```
