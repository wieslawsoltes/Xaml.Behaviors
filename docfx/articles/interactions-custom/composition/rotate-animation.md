# RotateAnimation

`RotateAnimation` provides helper methods for rotation animations using composition.

### Methods

| Method | Description |
| --- | --- |
| `SetRotateClockwise(Control element, double milliseconds)` | Rotates 360 degrees clockwise (0 to 360). |
| `SetRotateCounterClockwise(Control element, double milliseconds)` | Rotates 360 degrees counter-clockwise (0 to -360). |
| `SetRotateIn(Control element, double milliseconds)` | Rotates in from -180 to 0 degrees. |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

RotateAnimation.SetRotateClockwise(mySpinner, 2000);
```

**XAML**

You can use the methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <PathIcon Data="{StaticResource icons_settings}"
              RotateAnimation.RotateClockwise="2000" />
</UserControl>
```
