# FadeAnimation

`FadeAnimation` provides helper methods for simple opacity animations using composition.

### Methods

| Method | Description |
| --- | --- |
| `SetFadeIn(Control element, double milliseconds)` | Animates opacity from 0 to 1. |
| `SetFadeOut(Control element, double milliseconds)` | Animates opacity from 1 to 0. |
| `SetCustomFade(Control element, double fromOpacity, double toOpacity, double milliseconds)` | Animates opacity between specified values. |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

FadeAnimation.SetFadeIn(myImage, 500);
```

**XAML**

You can use the single-parameter methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Image Source="logo.png"
           FadeAnimation.FadeIn="500" />
</UserControl>
```

> **Note:** `SetCustomFade` cannot be used directly as an attached property in XAML because it requires multiple parameters.
