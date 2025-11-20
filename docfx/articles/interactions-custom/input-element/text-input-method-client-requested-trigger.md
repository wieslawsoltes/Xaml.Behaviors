# TextInputMethodClientRequestedTrigger

A trigger that executes when the input method client is requested on the associated `InputElement`.

## Usage

```xml
<TextBox Width="200">
    <Interaction.Behaviors>
        <TextInputMethodClientRequestedTrigger>
            <InvokeCommandAction Command="{Binding ClientRequestedCommand}" />
        </TextInputMethodClientRequestedTrigger>
    </Interaction.Behaviors>
</TextBox>
```
