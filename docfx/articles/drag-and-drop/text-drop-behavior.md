# TextDropBehavior

`TextDropBehavior` handles text drop operations. It validates that the dropped data contains text and executes a command with the text content.

## Properties

*   **`Command`**: The `ICommand` to execute when text is dropped. The command parameter will be the dropped text string.

## Usage

```xml
<TextBox>
    <Interaction.Behaviors>
        <TextDropBehavior Command="{Binding DropTextCommand}" />
    </Interaction.Behaviors>
</TextBox>
```
