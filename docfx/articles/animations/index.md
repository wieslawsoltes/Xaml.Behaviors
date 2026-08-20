# Standalone animations

`Xaml.Behaviors.Animations` is the reusable animation layer for this repository. It targets `net8.0` and `net10.0`, depends only on Avalonia, and does not reference `Xaml.Behaviors.Interactivity` or any interactions package.

Install it when an application or library needs the animation implementations without behavior, action, or trigger types:

```xml
<PackageReference Include="Xaml.Behaviors.Animations" Version="..." />
```

The repository includes an [animations-only sample application](https://github.com/wieslawsoltes/Xaml.Behaviors/tree/master/samples/AnimationsTestApplication) with a dedicated tab for every feature family. Its project references only Avalonia and `Xaml.Behaviors.Animations`.

## API groups

| Group | APIs |
| --- | --- |
| Avalonia key-frame animation | `AnimationFactory`, `AnimationRunner`, `FluidMoveAnimation`, `IAnimationBuilder` |
| Composition catalogs | `AttentionAnimations`, `EntranceAnimations`, `ExitAnimations`, `FramerMotionAnimations`, `SpecialAnimations` |
| Composition primitives | `FadeAnimation`, `RotateAnimation`, `ScaleAnimation`, `SlidingAnimation` |
| Reusable effects | `OrbitAnimation`, `ParallaxAnimation`, `TiltAnimation` |
| Selection animation | `SelectionIndicatorAnimation`, `SelectingItemsControlBehavior` attached property |
| Transitions | `TransitionOperations` |

The CLR namespace remains `Avalonia.Xaml.Interactions.Custom` for source and XAML compatibility with earlier releases. The types now live in the `Xaml.Behaviors.Animations` assembly, while `Xaml.Behaviors.Interactions.Custom` ships type forwarders for legacy assembly-qualified references.

## XAML composition animations

The composition catalogs are attached properties, so they can be used with no behavior collection:

```xml
<UserControl
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:animations="using:Avalonia.Xaml.Interactions.Custom">
  <Border
      Width="220"
      Height="150"
      animations:EntranceAnimations.FadeInUp="800" />
</UserControl>
```

The attached values are durations in milliseconds. The animation begins when the element receives a composition visual.

## Direct use from code

Create and run a normal Avalonia animation. Async variants return the completion task when callers need to coordinate follow-up work:

```csharp
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.Custom;

public static class WelcomeAnimation
{
    public static void Start(Control target)
    {
        var animation = AnimationFactory.CreateFadeIn(
            TimeSpan.FromMilliseconds(150),
            TimeSpan.FromMilliseconds(350));

        AnimationRunner.TryRun(animation, target);
    }
}
```

`AnimationRunner.TryBuildAndRunAsync` applies the same explicit-animation-first selection used by the behavior adapters and returns `null` when neither an animation nor a builder result is available. `FluidMoveAnimation.TryRun` prepares a control's translation transform before starting its movement animation.

Composition effects expose calculation and application separately where useful. For example, a scroll observer can call `ParallaxAnimation.Apply(target, offset, ratio)` without attaching `ParallaxBehavior`. `OrbitAnimation` maintains its orientation state for callers, while `TiltAnimation` calculates an orientation directly from the target size and pointer position.

`SelectionIndicatorAnimation.TryStart` can animate explicitly supplied indicator/container visuals or resolve the conventional `PART_SelectedPipe` from two templated item containers. `SelectingItemsControlBehavior.EnableSelectionAnimation` is the convenient attached-property adapter over the same primitive.

Transition collection operations are likewise independent:

```csharp
var transition = new DoubleTransition
{
    Property = Visual.OpacityProperty,
    Duration = TimeSpan.FromMilliseconds(200)
};

TransitionOperations.Add(target, transition);

using IDisposable subscription = TransitionOperations.Observe(
    target,
    transitions => OnTransitionsReplaced(transitions));
```

`Observe` reports the current collection immediately and subsequent replacements until the returned subscription is disposed.

## Behavior adapters

Install `Xaml.Behaviors.Interactions.Custom` when XAML attachment lifecycle, triggers, or actions are desired. Its animation and transition adapters reference `Xaml.Behaviors.Animations` and use the same shared implementation; existing behavior XAML remains unchanged.
