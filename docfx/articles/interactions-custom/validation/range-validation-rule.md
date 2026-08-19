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

All rule configuration properties are Avalonia styled properties, so they can use compiled bindings. When a bound rule property changes, the associated validation behavior immediately validates its current value again.

```xml
<SliderValidationBehavior IsValid="{Binding IsValid, Mode=OneWayToSource}">
  <RangeValidationRule x:TypeArguments="x:Double"
                       Minimum="{Binding Minimum}"
                       Maximum="{Binding Maximum}"
                       ErrorMessage="{Binding RangeErrorMessage}" />
</SliderValidationBehavior>
```
