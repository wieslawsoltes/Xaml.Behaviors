# ThemeVariantBehavior

Sets the `ThemeVariantScope.RequestedThemeVariant` on the associated control.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| ThemeVariant | `ThemeVariant` | Gets or sets the theme variant to assign. |

## Usage

```xml
<ThemeVariantScope>
    <Interaction.Behaviors>
        <ThemeVariantBehavior ThemeVariant="Dark" />
    </Interaction.Behaviors>
    <StackPanel>
        <TextBlock Text="This is Dark Theme" />
    </StackPanel>
</ThemeVariantScope>
```
