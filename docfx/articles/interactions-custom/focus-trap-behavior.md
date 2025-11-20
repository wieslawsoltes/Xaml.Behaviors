# FocusTrapBehavior

The `FocusTrapBehavior` traps keyboard focus within the attached element. This is essential for creating accessible modal dialogs, overlays, or any UI section where the user's focus should be constrained.

When active, pressing `Tab` or `Shift+Tab` will cycle focus only among the focusable descendants of the attached element. If focus attempts to leave the container, it is wrapped around to the beginning or end.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `IsActive` | `bool` | Gets or sets a value indicating whether the focus trap is active. Default is `true`. |

## Example

```xml
<Border BorderBrush="Red" BorderThickness="2" Padding="20">
    <StackPanel Spacing="10">
        <TextBlock Text="Trapped Area" />
        <TextBox Text="Input 1" />
        <Button Content="Action" />
        
        <Interaction.Behaviors>
            <FocusTrapBehavior IsActive="True" />
        </Interaction.Behaviors>
    </StackPanel>
</Border>
```

## Notes

-   The behavior listens to the `KeyDown` event on the attached control to intercept `Tab` keys.
-   It uses Avalonia's `KeyboardNavigationHandler` to determine the next focusable element.
-   If the user clicks outside the trapped area, focus might escape. This behavior primarily handles keyboard navigation. For strict modality, consider using a modal `Window` or overlay that consumes pointer events outside the content.
