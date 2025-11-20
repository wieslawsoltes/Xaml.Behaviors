# FramerMotionAnimations

`FramerMotionAnimations` provides animations inspired by the Framer Motion library for React.

### Methods

| Method | Description |
| --- | --- |
| `SetFadeIn(Control element, double milliseconds)` | Fades in. |
| `SetFadeInUp(Control element, double milliseconds)` | Fades in moving up. |
| `SetFadeInDown(Control element, double milliseconds)` | Fades in moving down. |
| `SetFadeInLeft(Control element, double milliseconds)` | Fades in moving right. |
| `SetFadeInRight(Control element, double milliseconds)` | Fades in moving left. |
| `SetFadeOut(Control element, double milliseconds)` | Fades out. |
| `SetFadeOutUp(Control element, double milliseconds)` | Fades out moving up. |
| `SetFadeOutDown(Control element, double milliseconds)` | Fades out moving down. |
| `SetFadeOutLeft(Control element, double milliseconds)` | Fades out moving left. |
| `SetFadeOutRight(Control element, double milliseconds)` | Fades out moving right. |
| `SetScaleIn(Control element, double milliseconds)` | Scales in. |
| `SetScaleOut(Control element, double milliseconds)` | Scales out. |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

FramerMotionAnimations.SetFadeInUp(myCard, 600);
```

**XAML**

You can use the methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Border Background="White"
            FramerMotionAnimations.FadeInUp="600" />
</UserControl>
```
