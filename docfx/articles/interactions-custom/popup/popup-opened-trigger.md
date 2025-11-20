# PopupOpenedTrigger

Executes actions when the associated `Popup` raises the `Opened` event.

### Usage Example

```xml
<Popup>
    <Interaction.Behaviors>
        <PopupOpenedTrigger>
            <InvokeCommandAction Command="{Binding PopupOpenedCommand}" />
        </PopupOpenedTrigger>
    </Interaction.Behaviors>
    <!-- Content -->
</Popup>
```
