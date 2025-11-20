# SetCursorFromProviderBehavior

Sets the cursor of the associated `InputElement` by retrieving it from an `ICursorProvider`.

### Properties
- `CursorProvider`: An object implementing `ICursorProvider`.

### Usage Example

```xml
<Border Background="LightGreen">
    <Interaction.Behaviors>
        <SetCursorFromProviderBehavior CursorProvider="{Binding MyCursorProvider}" />
    </Interaction.Behaviors>
</Border>
```
