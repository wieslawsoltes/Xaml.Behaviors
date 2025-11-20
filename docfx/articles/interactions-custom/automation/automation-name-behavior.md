# AutomationNameBehavior

This behavior allows you to bind the `AutomationProperties.Name` attached property to a value in your ViewModel or another source. While you can often bind `AutomationProperties.Name` directly in XAML, this behavior provides an alternative way to inject this property via behaviors, which can be useful in styles or templates where direct binding might be cumbersome or conditional.

### Properties
*   `AutomationName`: The string value to set as the automation name.

### Example

```xml
<TextBox Text="{Binding UserName}">
    <Interaction.Behaviors>
        <AutomationNameBehavior AutomationName="{Binding UserNameError}" />
    </Interaction.Behaviors>
</TextBox>
```
