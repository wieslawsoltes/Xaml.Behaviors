# SlidingAnimation

`SlidingAnimation` provides helper methods for sliding animations (translation) relative to the element's size.

### Methods

| Method | Description |
| --- | --- |
| `SetLeft(Control element, double milliseconds)` | Slides from left (offset -Width to 0). |
| `SetRight(Control element, double milliseconds)` | Slides from right (offset 2*Width to 0). |
| `SetTop(Control element, double milliseconds)` | Slides from top (offset -Height to 0). |
| `SetBottom(Control element, double milliseconds)` | Slides from bottom (offset 2*Height to 0). |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

SlidingAnimation.SetLeft(mySidebar, 400);
```

**XAML**

You can use the methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Grid Background="Gray"
          SlidingAnimation.Left="400" />
</UserControl>
```
