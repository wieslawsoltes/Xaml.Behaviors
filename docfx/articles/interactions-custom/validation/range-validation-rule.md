# RangeValidationRule

Validation rule that checks whether a value is within a specified range.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Minimum | `T` | Gets or sets the minimum allowed value. |
| Maximum | `T` | Gets or sets the maximum allowed value. |
| ErrorMessage | `string` | Gets or sets the error message for failed validation. |

## Usage

```xml
<RangeValidationRule Minimum="0" Maximum="100" ErrorMessage="Value must be between 0 and 100." />
```
