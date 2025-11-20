# ActualThemeVariantChangedBehavior

`ActualThemeVariantChangedBehavior<T>` is an abstract base class for behaviors that need to react when the `ActualThemeVariantChanged` event is raised on the associated `StyledElement`.

### Usage

Inherit from this class and implement the `OnActualThemeVariantChangedEventOverride` method.

```csharp
public class MyThemeBehavior : ActualThemeVariantChangedBehavior<Control>
{
    protected override IDisposable OnActualThemeVariantChangedEventOverride()
    {
        // Handle theme variant changed
        return Disposable.Empty;
    }
}
```
