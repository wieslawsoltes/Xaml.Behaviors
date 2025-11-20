# ExitAnimations

`ExitAnimations` is a static helper class that provides exit animations (disappearing from screen) inspired by libraries like animate.css.

### Methods

All methods take a `Control` to animate and a duration in milliseconds.

| Method | Description |
| --- | --- |
| `SetBackOutDown` | Exits to bottom with a back effect. |
| `SetBackOutLeft` | Exits to left with a back effect. |
| `SetBackOutRight` | Exits to right with a back effect. |
| `SetBackOutUp` | Exits to top with a back effect. |
| `SetBounceOut` | Exits with a bounce effect. |
| `SetBounceOutDown` | Exits to bottom with a bounce effect. |
| `SetBounceOutLeft` | Exits to left with a bounce effect. |
| `SetBounceOutRight` | Exits to right with a bounce effect. |
| `SetBounceOutUp` | Exits to top with a bounce effect. |
| `SetFadeOut` | Simple fade out. |
| `SetFadeOutDown` | Fades out moving down. |
| `SetFadeOutDownBig` | Fades out moving down (larger distance). |
| `SetFadeOutLeft` | Fades out moving left. |
| `SetFadeOutLeftBig` | Fades out moving left (larger distance). |
| `SetFadeOutRight` | Fades out moving right. |
| `SetFadeOutRightBig` | Fades out moving right (larger distance). |
| `SetFadeOutUp` | Fades out moving up. |
| `SetFadeOutUpBig` | Fades out moving up (larger distance). |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

// In your code-behind or behavior
ExitAnimations.SetFadeOutDown(myControl, 800);
```

**XAML**

You can use the methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Border Background="Red"
            ExitAnimations.FadeOutDown="800" />
</UserControl>
```
