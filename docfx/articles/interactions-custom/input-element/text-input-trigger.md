# TextInputTrigger

A trigger that executes when text input is received on the associated `InputElement`.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Text | `string` | Gets or sets the text that triggers the action. |

## Usage

```xml
<TextBox Width="200">
    <Interaction.Behaviors>
        <TextInputTrigger Text="Hello">
            <InvokeCommandAction Command="{Binding HelloTypedCommand}" />
        </TextInputTrigger>
    </Interaction.Behaviors>
</TextBox>
```
