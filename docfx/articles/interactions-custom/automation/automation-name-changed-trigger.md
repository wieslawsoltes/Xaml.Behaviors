# AutomationNameChangedTrigger

This trigger listens for changes to the `AutomationProperties.Name` property on the associated control. This is particularly useful for accessibility-driven logic, where you might want to perform an action (like logging or updating another control) when the accessible name of an element changes.

### Example

```xml
<TextBlock Name="StatusText" Text="Ready" AutomationProperties.Name="Ready State">
    <Interaction.Behaviors>
        <AutomationNameChangedTrigger>
            <CallMethodAction TargetObject="{Binding}" MethodName="LogAccessibilityChange" />
        </AutomationNameChangedTrigger>
    </Interaction.Behaviors>
</TextBlock>
```
