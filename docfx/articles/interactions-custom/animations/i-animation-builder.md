# IAnimationBuilder

The `IAnimationBuilder` interface allows for dynamic animation creation.

```csharp
public interface IAnimationBuilder
{
    Animation.Animation? Build(Control control);
}
```

You can implement this interface to create complex animations based on the control's state or properties at runtime.
