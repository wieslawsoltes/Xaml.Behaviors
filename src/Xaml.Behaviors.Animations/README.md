# Xaml.Behaviors.Animations

Reusable Avalonia key-frame animations, composition animation catalogs and effects, animation runners/builders, selection animation, and transition operations. The package depends only on Avalonia and can be installed without XAML Behaviors or Interactivity.

```xml
<PackageReference Include="Xaml.Behaviors.Animations" Version="..." />
```

The CLR namespace remains `Avalonia.Xaml.Interactions.Custom` for compatibility with existing code and XAML. Behavior, action, and trigger adapters are available separately from `Xaml.Behaviors.Interactions.Custom`, use this shared implementation, and provide type forwarders for legacy assembly-qualified references.

See the [standalone animations documentation](https://wieslawsoltes.github.io/Xaml.Behaviors/articles/animations/) for the API groups and examples.
