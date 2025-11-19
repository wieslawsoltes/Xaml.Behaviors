# Clipboard Actions

The `Xaml.Behaviors.Interactions` package provides several actions for interacting with the system clipboard. These actions allow you to get, set, and clear clipboard data directly from XAML.

## Available Actions

*   **`ClearClipboardAction`**: Clears the contents of the clipboard.
*   **`GetClipboardDataAction`**: Retrieves data from the clipboard in a specified format.
*   **`GetClipboardFormatsAction`**: Retrieves a list of available data formats from the clipboard.
*   **`GetClipboardTextAction`**: Retrieves text from the clipboard.
*   **`SetClipboardDataObjectAction`**: Sets a data object to the clipboard.
*   **`SetClipboardTextAction`**: Sets text to the clipboard.

## Common Properties

Most clipboard actions expose a `Clipboard` property which allows you to inject a specific `IClipboard` implementation. If not set, the action attempts to resolve the clipboard from the `TopLevel` of the associated object.

## Usage Examples

### Setting Clipboard Text

```xml
<Button Content="Copy to Clipboard">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <SetClipboardTextAction Text="Hello from Avalonia!" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

### Getting Clipboard Text

```xml
<Button Content="Paste from Clipboard">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <GetClipboardTextAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

### Clearing Clipboard

```xml
<Button Content="Clear Clipboard">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <ClearClipboardAction />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```
