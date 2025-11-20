# ThemeVariantTrigger

Executes actions when the associated control's `StyledElement.ActualThemeVariant` matches the specified `ThemeVariant`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| ThemeVariant | `ThemeVariant` | Gets or sets the theme variant to watch for. |

## Usage

```xml
<StackPanel>
    <Interaction.Behaviors>
        <ThemeVariantTrigger ThemeVariant="Dark">
            <ChangePropertyAction TargetObject="{Binding #MyText}" PropertyName="Text" Value="Dark Mode Active" />
        </ThemeVariantTrigger>
        <ThemeVariantTrigger ThemeVariant="Light">
            <ChangePropertyAction TargetObject="{Binding #MyText}" PropertyName="Text" Value="Light Mode Active" />
        </ThemeVariantTrigger>
    </Interaction.Behaviors>
    <TextBlock x:Name="MyText" />
</StackPanel>
```
