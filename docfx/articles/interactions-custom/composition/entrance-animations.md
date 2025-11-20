# EntranceAnimations

`EntranceAnimations` is a static helper class that provides entrance animations (appearing on screen) inspired by libraries like animate.css.

### Methods

All methods take a `Control` to animate and a duration in milliseconds.

| Method | Description |
| --- | --- |
| `SetBackInDown` | Enters from top with a back effect. |
| `SetBackInLeft` | Enters from left with a back effect. |
| `SetBackInRight` | Enters from right with a back effect. |
| `SetBackInUp` | Enters from bottom with a back effect. |
| `SetBounceIn` | Enters with a bounce effect. |
| `SetBounceInDown` | Enters from top with a bounce effect. |
| `SetBounceInLeft` | Enters from left with a bounce effect. |
| `SetBounceInRight` | Enters from right with a bounce effect. |
| `SetBounceInUp` | Enters from bottom with a bounce effect. |
| `SetFadeIn` | Simple fade in. |
| `SetFadeInDown` | Fades in moving down. |
| `SetFadeInDownBig` | Fades in moving down (larger distance). |
| `SetFadeInLeft` | Fades in moving right (from left). |
| `SetFadeInLeftBig` | Fades in moving right (from left, larger distance). |
| `SetFadeInRight` | Fades in moving left (from right). |
| `SetFadeInRightBig` | Fades in moving left (from right, larger distance). |
| `SetFadeInUp` | Fades in moving up. |
| `SetFadeInUpBig` | Fades in moving up (larger distance). |

### Example

**C#**

```csharp
using Avalonia.Xaml.Interactions.Custom;

// In your code-behind or behavior
EntranceAnimations.SetFadeInUp(myControl, 800);
```

**XAML**

You can use the methods as attached properties in XAML. The value is the duration in milliseconds.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Border Background="Red"
            EntranceAnimations.FadeInUp="800" />
</UserControl>
```
