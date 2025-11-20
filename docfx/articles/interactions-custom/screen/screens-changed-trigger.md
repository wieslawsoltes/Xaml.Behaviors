# ScreensChangedTrigger

`ScreensChangedTrigger` is a trigger that executes its actions when the screen configuration changes (e.g., a monitor is added or removed, or resolution changes).

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Screens | `Screens` | Gets or sets the `Screens` instance that is observed. If not set, the screens of the associated `TopLevel` will be used. |

## Usage

```xml
<Window>
    <Interaction.Behaviors>
        <ScreensChangedTrigger>
            <InvokeCommandAction Command="{Binding UpdateLayoutCommand}" />
        </ScreensChangedTrigger>
    </Interaction.Behaviors>
</Window>
```
