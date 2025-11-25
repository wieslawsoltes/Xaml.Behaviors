# Typed Property Trigger

The `[GeneratePropertyTrigger]` attribute generates a strongly-typed trigger that listens to an Avalonia property via `GetObservable` and executes actions when the property's value matches your expected value. This avoids XAML bindings and reflection for scenarios like reacting to `IsVisible`, `Bounds`, or other control properties.

## Usage

Annotate an Avalonia property field (or use the assembly-level form) to generate a trigger.

```csharp
using Avalonia;
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

public class MyControl : Control
{
    public static readonly StyledProperty<double> OpacityProperty =
        AvaloniaProperty.Register<MyControl, double>(nameof(Opacity));
}

[assembly: GeneratePropertyTrigger(typeof(MyControl), "OpacityProperty")]
```

The generator produces `OpacityPropertyTrigger` in the target type’s namespace (e.g., `MyApp.Controls`), prefixing the type name for assembly-level attributes.

### XAML Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.Controls">
  <Grid>
    <Interaction.Behaviors>
      <local:OpacityPropertyTrigger Value="0" ComparisonCondition="GreaterThan">
        <local:SomeAction />
      </local:OpacityPropertyTrigger>
    </Interaction.Behaviors>
    <!-- UI content -->
  </Grid>
</UserControl>
```

## Options

- `UseDispatcher`: marshal evaluation and action invocation to the UI thread.
- `Name`: optional override for the generated class name.
- `SourceName`: resolve the source by name from the nearest `NameScope`; falls back to `SourceObject` or `AssociatedObject`. If the target type is not part of the logical tree, the generator emits a warning because `SourceName` cannot be resolved. Types implementing `Avalonia.LogicalTree.ILogical` (e.g., controls) are considered part of the logical tree.

## Diagnostics

- `XBG019`: The target member is not an Avalonia styled or direct property.
- Standard accessibility/static/generic diagnostics from other generators apply.

## Notes

- The trigger exposes `Value` (typed) and `ComparisonCondition` properties, and evaluates once on attach.
- The initial evaluation includes `null` values so comparisons against `Value` run even when the property starts as `null`.
- Assembly-level generation prefixes the target type name to avoid collisions.
