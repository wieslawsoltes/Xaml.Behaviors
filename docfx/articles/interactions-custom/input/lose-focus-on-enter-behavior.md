# LoseFocusOnEnterBehavior

`LoseFocusOnEnterBehavior` makes the associated control lose focus when the Enter key is pressed.

## Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <TextBox Text="Press Enter to lose focus">
        <Interaction.Behaviors>
            <LoseFocusOnEnterBehavior />
        </Interaction.Behaviors>
    </TextBox>
</UserControl>
```
