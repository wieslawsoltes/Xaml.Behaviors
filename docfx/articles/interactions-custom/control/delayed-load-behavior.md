# DelayedLoadBehavior

This behavior hides the associated control initially and then makes it visible after a specified delay. This can be useful for staggering the appearance of UI elements.

### Properties
*   `Delay`: The time to wait before showing the control (default: 500ms).

### Example

```xml
<TextBlock Text="I appear later!">
    <Interaction.Behaviors>
        <DelayedLoadBehavior Delay="00:00:02" />
    </Interaction.Behaviors>
</TextBlock>
```
