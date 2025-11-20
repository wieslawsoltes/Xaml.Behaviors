# ClipboardMonitorBehavior

The `ClipboardMonitorBehavior` monitors the system clipboard for specific data formats and updates a `HasData` property. This is useful for enabling or disabling UI elements (like a "Paste" button) based on whether compatible data is available on the clipboard.

## Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| `Formats` | `string` | A comma-separated list of clipboard formats to listen for (e.g., "Text,FileNames"). Default is "Text". |
| `HasData` | `bool` | Read-only property indicating whether the clipboard contains data in any of the specified formats. |

## Events

| Event | Description |
| :--- | :--- |
| `ContentChanged` | Occurs when the clipboard content availability changes (i.e., `HasData` changes). |

## Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <StackPanel Spacing="10">
        <Button Content="Paste" 
                IsEnabled="{Binding #ClipboardMonitor.HasData}"
                Command="{Binding PasteCommand}" />

        <Interaction.Behaviors>
            <ClipboardMonitorBehavior Name="ClipboardMonitor" Formats="Text" />
        </Interaction.Behaviors>
    </StackPanel>
</UserControl>
```

## Notes

- The behavior polls the clipboard every second to check for format availability.
- It does not read the actual content unless you explicitly access the clipboard in your code.
- `HasData` will be true if *any* of the specified formats are present.
