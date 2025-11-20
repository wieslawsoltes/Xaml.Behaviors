# AttentionAnimations

`AttentionAnimations` is a static helper class that provides attention-seeking animations inspired by libraries like animate.css. These animations are typically applied to a control when it is loaded or in response to an event.

### Methods

All methods take a `Control` to animate and a duration in milliseconds.

| Method | Description |
| --- | --- |
| `SetBounce` | Applies a bounce animation. |
| `SetFlash` | Applies a flash animation (opacity toggling). |
| `SetPulse` | Applies a pulse animation (scaling). |
| `SetRubberBand` | Applies a rubber band animation (scaling). |
| `SetShakeX` | Applies a horizontal shake animation. |
| `SetShakeY` | Applies a vertical shake animation. |
| `SetHeadShake` | Applies a head shake animation (rotation and translation). |
| `SetSwing` | Applies a swing animation (rotation). |
| `SetTada` | Applies a "tada" animation (scaling and rotation). |
| `SetWobble` | Applies a wobble animation (translation and rotation). |
| `SetJello` | Applies a jello animation (skewing/transform). |
| `SetHeartBeat` | Applies a heartbeat animation (scaling). |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

// In your code-behind or behavior
AttentionAnimations.SetBounce(myButton, 1000);
```

**XAML**

You can use the methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Button Content="Bounce Me"
            AttentionAnimations.Bounce="1000" />
</UserControl>
```
