# IAnimationBuilder

The `IAnimationBuilder` interface allows for dynamic animation creation. It is shipped by `Xaml.Behaviors.Animations` and has no behaviors dependency; the animation behaviors and actions consume the same interface.

```csharp
public interface IAnimationBuilder
{
    Animation.Animation? Build(Control control);
}
```

You can implement this interface to create complex animations based on the control's state or properties at runtime.
