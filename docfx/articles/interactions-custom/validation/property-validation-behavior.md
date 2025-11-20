# PropertyValidationBehavior

Base behavior that validates a property value using a set of rules.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Property | `AvaloniaProperty` | Gets or sets the property to validate. |
| Rules | `AvaloniaList<IValidationRule<TValue>>` | Gets validation rules collection. |
| IsValid | `bool` | Gets or sets value indicating whether the property value is valid. |
| Error | `string` | Gets or sets the validation error message. |

## Remarks

This behavior is intended to be subclassed for specific controls and properties.


## Properties

| Property | Type | Description |
| --- | --- | --- |
| Property | `AvaloniaProperty` | Gets or sets the property to validate. This is an avalonia property. |
| IsValid | `bool` | Gets or sets value indicating whether the property value is valid. This is an avalonia property. |
| Error | `string` | Gets or sets the validation error message. This is an avalonia property. |
| Rules | `AvaloniaList<IValidationRule<TValue>>` | Gets validation rules collection. This is an avalonia property. |
