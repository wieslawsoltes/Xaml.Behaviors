# MinLengthValidationRule

Validation rule that requires a string with a minimal length.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Length | `int` | Gets or sets the minimal allowed length. |
| ErrorMessage | `string` | Gets or sets the error message for failed validation. |

## Usage

```xml
<MinLengthValidationRule Length="5" ErrorMessage="Text must be at least 5 characters long." />
```
