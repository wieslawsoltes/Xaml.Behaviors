# MaxValueValidationRule

Validation rule that checks that a numeric value is less than or equal to a maximum value.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| MaxValue | `T` | Gets or sets the maximum value. |
| ErrorMessage | `string` | Gets or sets the error message for failed validation. |

## Usage

```xml
<MaxValueValidationRule MaxValue="100" ErrorMessage="Value must be less than 100." />
```
