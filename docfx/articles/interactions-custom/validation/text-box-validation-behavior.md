# TextBoxValidationBehavior

Validation behavior for `TextBox` text.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Property | `AvaloniaProperty` | Gets or sets the property to validate. This is an avalonia property. |
| IsValid | `bool` | Gets or sets value indicating whether the property value is valid. This is an avalonia property. |
| Error | `string` | Gets or sets the validation error message. This is an avalonia property. |
| Rules | `AvaloniaList<IValidationRule<string>>` | Gets validation rules collection. This is an avalonia property. |

## Usage

```xml
<TextBox x:Name="TextBox" Text="">
    <Interaction.Behaviors>
        <TextBoxValidationBehavior>
            <RequiredTextValidationRule ErrorMessage="Text is required." />
            <MinLengthValidationRule Length="5" ErrorMessage="Text must be at least 5 characters long." />
        </TextBoxValidationBehavior>
    </Interaction.Behaviors>
</TextBox>
```
