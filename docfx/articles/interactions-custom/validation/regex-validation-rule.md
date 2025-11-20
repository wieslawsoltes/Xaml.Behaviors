# RegexValidationRule

Validation rule that checks value against a regular expression pattern.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Pattern | `string` | Gets or sets the regex pattern. |
| ErrorMessage | `string` | Gets or sets the error message for failed validation. |

## Usage

```xml
<RegexValidationRule Pattern="^\d+$" ErrorMessage="Value must be a number." />
```
