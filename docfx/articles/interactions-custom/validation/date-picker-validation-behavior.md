# DatePickerValidationBehavior

Validation behavior for `DatePicker` selected date.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Property | `AvaloniaProperty` | Gets or sets the property to validate. This is an avalonia property. |
| IsValid | `bool` | Gets or sets value indicating whether the property value is valid. This is an avalonia property. |
| Error | `string` | Gets or sets the validation error message. This is an avalonia property. |
| Rules | `AvaloniaList<IValidationRule<DateTimeOffset?>>` | Gets validation rules collection. This is an avalonia property. |

## Usage

```xml
<DatePicker x:Name="DatePicker">
    <Interaction.Behaviors>
        <DatePickerValidationBehavior>
            <RequiredDateValidationRule ErrorMessage="Date is required." />
        </DatePickerValidationBehavior>
    </Interaction.Behaviors>
</DatePicker>
```
