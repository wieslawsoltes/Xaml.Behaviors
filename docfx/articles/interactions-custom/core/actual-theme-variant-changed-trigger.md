# ActualThemeVariantChangedTrigger

The `ActualThemeVariantChangedTrigger` executes its actions when the `ActualThemeVariantChanged` event is raised on the associated `StyledElement`. This is useful for reacting to theme changes (e.g., Light/Dark mode).

### Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Interaction.Behaviors>
        <ActualThemeVariantChangedTrigger>
            <InvokeCommandAction Command="{Binding ThemeChangedCommand}" />
        </ActualThemeVariantChangedTrigger>
    </Interaction.Behaviors>
</UserControl>
```
