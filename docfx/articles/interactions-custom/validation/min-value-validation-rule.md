# MinValueValidationRule

Validation rule that checks that a numeric value is greater than or equal to a minimum value.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| MinValue | `T` | Gets or sets the minimum value. |
| ErrorMessage | `string` | Gets or sets the error message for failed validation. |

## Usage

```xml
<MinValueValidationRule MinValue="0" ErrorMessage="Value must be positive." />
```
