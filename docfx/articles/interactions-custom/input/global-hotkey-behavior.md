# GlobalHotkeyBehavior

The `GlobalHotkeyBehavior` registers a global hotkey (window-wide) to execute actions. It attaches to the `TopLevel` (Window) of the associated control and listens for key events, allowing hotkeys to work regardless of which control has focus within the window.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Key | Key | The key to listen for. |
| KeyModifiers | KeyModifiers | The key modifiers to listen for. |

## Usage

```xml
<UserControl>
    <StackPanel>
        <Interaction.Behaviors>
            <GlobalHotkeyBehavior Key="H" KeyModifiers="Control">
                <iChangePropertyAction TargetObject="{Binding #StatusText}" 
                                       PropertyName="Text" 
                                       Value="Hotkey Pressed!" />
            </GlobalHotkeyBehavior>
        </Interaction.Behaviors>
        
        <TextBlock Name="StatusText" Text="Waiting..." />
    </StackPanel>
</UserControl>
```

## Remarks

This behavior uses `TopLevel.AddHandler` with `RoutingStrategies.Tunnel` to intercept key events at the window level. It marks the event as handled when the hotkey is triggered.
