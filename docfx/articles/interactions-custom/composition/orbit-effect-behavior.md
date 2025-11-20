# OrbitEffectBehavior

`OrbitEffectBehavior` allows rotating the attached control in 3D space using pointer manipulation.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| Sensitivity | double | Gets or sets the sensitivity of the rotation. Default is 0.5. |

## Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Border Background="LightGreen" Width="200" Height="200">
        <Interaction.Behaviors>
            <iOrbitEffectBehavior Sensitivity="0.8" />
        </Interaction.Behaviors>
        <TextBlock Text="Drag me!" HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Border>
</UserControl>
```
