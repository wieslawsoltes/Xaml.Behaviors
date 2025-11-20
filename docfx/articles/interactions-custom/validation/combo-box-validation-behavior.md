# ComboBoxValidationBehavior

Validation behavior for `ComboBox` selected item.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Property | `AvaloniaProperty` | Gets or sets the property to validate. This is an avalonia property. |
| IsValid | `bool` | Gets or sets value indicating whether the property value is valid. This is an avalonia property. |
| Error | `string` | Gets or sets the validation error message. This is an avalonia property. |
| Rules | `AvaloniaList<IValidationRule<object>>` | Gets validation rules collection. This is an avalonia property. |

## Usage

```xml
<ComboBox x:Name="ComboBox" SelectedIndex="0">
    <ComboBoxItem>Item 1</ComboBoxItem>
    <ComboBoxItem>Item 2</ComboBoxItem>
    <ComboBoxItem>Item 3</ComboBoxItem>
    <Interaction.Behaviors>
        <ComboBoxValidationBehavior>
            <NotNullValidationRule ErrorMessage="Selection is required." />
        </ComboBoxValidationBehavior>
    </Interaction.Behaviors>
</ComboBox>
```
