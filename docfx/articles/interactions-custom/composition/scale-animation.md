# ScaleAnimation

`ScaleAnimation` provides helper methods for scaling animations using composition.

### Methods

| Method | Description |
| --- | --- |
| `SetScaleIn(Control element, double milliseconds)` | Scales from 0 to 1. |
| `SetScaleOut(Control element, double milliseconds)` | Scales from 1 to 0. |
| `SetZoomIn(Control element, double milliseconds)` | Zooms in from 1.5 to 1. |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

ScaleAnimation.SetScaleIn(myPopup, 300);
```

**XAML**

You can use the methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Border Background="White"
            ScaleAnimation.ScaleIn="300" />
</UserControl>
```
