# ActiveScreenBehavior

`ActiveScreenBehavior` is a behavior that exposes the screen containing the associated `TopLevel` (e.g., Window). It updates the `ActiveScreen` property whenever the window moves or the screen configuration changes.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| ActiveScreen | `Screen` | Gets the active screen of the associated `TopLevel`. This is a read-only property. |

## Usage

```xml
<Window>
    <Interaction.Behaviors>
        <ActiveScreenBehavior x:Name="ActiveScreenBehavior" />
    </Interaction.Behaviors>

    <TextBlock Text="{Binding ActiveScreen.Scaling, ElementName=ActiveScreenBehavior, StringFormat='Scaling: {0}'}" />
</Window>
```
