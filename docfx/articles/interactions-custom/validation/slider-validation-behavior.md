# SliderValidationBehavior

Validation behavior for range based controls like `Slider` value.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Property | `AvaloniaProperty` | Gets or sets the property to validate. This is an avalonia property. |
| IsValid | `bool` | Gets or sets value indicating whether the property value is valid. This is an avalonia property. |
| Error | `string` | Gets or sets the validation error message. This is an avalonia property. |
| Rules | `AvaloniaList<IValidationRule<double>>` | Gets validation rules collection. This is an avalonia property. |

## Usage

```xml
<Slider x:Name="Slider" Value="0">
    <Interaction.Behaviors>
        <SliderValidationBehavior>
            <RangeValidationRule Minimum="0" Maximum="100" ErrorMessage="Value must be between 0 and 100." />
        </SliderValidationBehavior>
    </Interaction.Behaviors>
</Slider>
```
