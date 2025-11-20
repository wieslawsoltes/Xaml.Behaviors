# NumericUpDownValidationBehavior

Validation behavior for `NumericUpDown` value.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Property | `AvaloniaProperty` | Gets or sets the property to validate. This is an avalonia property. |
| IsValid | `bool` | Gets or sets value indicating whether the property value is valid. This is an avalonia property. |
| Error | `string` | Gets or sets the validation error message. This is an avalonia property. |
| Rules | `AvaloniaList<IValidationRule<decimal?>>` | Gets validation rules collection. This is an avalonia property. |

## Usage

```xml
<NumericUpDown x:Name="NumericUpDown" Value="0">
    <Interaction.Behaviors>
        <NumericUpDownValidationBehavior>
            <RequiredDecimalValidationRule ErrorMessage="Value is required." />
            <MinValueValidationRule MinValue="0" ErrorMessage="Value must be positive." />
            <MaxValueValidationRule MaxValue="100" ErrorMessage="Value must be less than 100." />
        </NumericUpDownValidationBehavior>
    </Interaction.Behaviors>
</NumericUpDown>
```
