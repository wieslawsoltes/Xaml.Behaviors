# ButtonHidePopupOnClickBehavior

Similar to `ButtonHideFlyoutOnClickBehavior`, but designed for buttons inside a `Popup`.

### Example

```xml
<Popup>
    <Border>
        <Button Content="Close Popup">
            <Interaction.Behaviors>
                <ButtonHidePopupOnClickBehavior />
            </Interaction.Behaviors>
        </Button>
    </Border>
</Popup>
```
