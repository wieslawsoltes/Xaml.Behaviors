# GotFocusTrigger

A trigger that executes when the associated `InputElement` receives focus.

## Usage

```xml
<TextBox Width="200">
    <Interaction.Behaviors>
        <GotFocusTrigger>
            <InvokeCommandAction Command="{Binding GotFocusCommand}" />
        </GotFocusTrigger>
    </Interaction.Behaviors>
</TextBox>
```
