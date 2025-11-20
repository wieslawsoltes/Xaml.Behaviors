# TiltEffectBehavior

A behavior that applies a 3D tilt rotation to the element based on the pointer position relative to the element's center.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| TiltStrength | double | The maximum tilt angle in degrees. Default is 5.0. |

## Usage

```xml
<Border Width="200" Height="120" Background="Blue">
    <Interaction.Behaviors>
        <TiltEffectBehavior TiltStrength="10" />
    </Interaction.Behaviors>
    <TextBlock Text="Hover Me" />
</Border>
```
