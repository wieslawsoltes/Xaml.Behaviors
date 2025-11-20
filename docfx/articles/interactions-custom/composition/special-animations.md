# SpecialAnimations

`SpecialAnimations` provides miscellaneous animations inspired by animate.css.

### Methods

| Method | Description |
| --- | --- |
| `SetFlip(Control element, double milliseconds)` | Applies a flip animation. |
| `SetFlipInX(Control element, double milliseconds)` | Flips in around the X axis. |
| `SetFlipInY(Control element, double milliseconds)` | Flips in around the Y axis. |
| `SetFlipOutX(Control element, double milliseconds)` | Flips out around the X axis. |
| `SetFlipOutY(Control element, double milliseconds)` | Flips out around the Y axis. |
| `SetHinge(Control element, double milliseconds)` | Applies a hinge animation (swinging and falling). |
| `SetJackInTheBox(Control element, double milliseconds)` | Applies a "Jack in the Box" animation. |
| `SetRollIn(Control element, double milliseconds)` | Rolls in the element. |
| `SetRollOut(Control element, double milliseconds)` | Rolls out the element. |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

SpecialAnimations.SetJackInTheBox(mySurprise, 1000);
```

**XAML**

You can use the methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Image Source="gift.png"
           SpecialAnimations.JackInTheBox="1000" />
</UserControl>
```
