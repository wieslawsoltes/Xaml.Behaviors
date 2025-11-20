# LostFocusTrigger

A trigger that executes when the associated `InputElement` loses focus.

## Usage

```xml
<TextBox Width="200">
    <Interaction.Behaviors>
        <LostFocusTrigger>
            <InvokeCommandAction Command="{Binding LostFocusCommand}" />
        </LostFocusTrigger>
    </Interaction.Behaviors>
</TextBox>
```
