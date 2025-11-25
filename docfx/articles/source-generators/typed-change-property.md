# Typed ChangePropertyAction

The `[GenerateTypedChangePropertyAction]` attribute generates a strongly-typed `Action` class that sets a specific property on a target object. This is the AOT-safe alternative to `ChangePropertyAction`.

## Usage

Annotate a property in your ViewModel (or any class) with `[GenerateTypedChangePropertyAction]`.

```csharp
public partial class MyViewModel
{
    [GenerateTypedChangePropertyAction]
    public string StatusText { get; set; }
}
```

The generator will create a class named `SetStatusTextAction` in the same namespace.

Assembly-level attributes are supported as well. The `propertyName` parameter can be a literal name, a wildcard (`*`) pattern, or a regular expression. Classes generated from assembly attributes are prefixed with the target type to reduce collisions (e.g. `TextBlockSetTextAction`).

```csharp
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Controls.TextBlock), "Text")]
[assembly: GenerateTypedChangePropertyAction(typeof(MyApp.ViewModels.ShellViewModel), "*Property")]
[assembly: GenerateTypedChangePropertyAction(typeof(MyApp.ViewModels.ShellViewModel), "^(Status|Title)$")]
```

> Only public or internal properties with accessible setters are supported. Internal members are fine when you generate inside the same assembly (the normal case). If you need to reach internal members in another assembly, that assembly must grant `InternalsVisibleTo("<your assembly name>")` to the consuming assembly. Wildcard/regex assembly attributes ignore inaccessible matches; if no accessible properties match, a diagnostic is produced and no action class is generated.
> Property types must be public, or internal and visible to the consuming assembly via the same `InternalsVisibleTo` arrangement; otherwise an XBG014 diagnostic is produced.
> Generated action classes are `public` unless the target or property type requires `internal`.

## UI Thread Dispatching

Set `UseDispatcher = true` on the attribute when the property assignment must run on the UI thread. The generated action queues the setter via `Dispatcher.UIThread.Post`; the default `false` keeps the immediate assignment.

```csharp
[GenerateTypedChangePropertyAction(UseDispatcher = true)]
public string StatusText { get; set; }
```

### Matching rules at a glance

| Aspect | Behavior |
| --- | --- |
| Pattern kinds | Literal name, `*` wildcard, or regular expression |
| Name collisions | Assembly attributes prefix the target type name (e.g. `TextBlockSetTextAction`) |
| Accessibility | Property and setter must be public/internal; property type must be public |
| Ambiguity | Multiple properties with the same name are treated as a single match; inaccessible setters are skipped |
| Inaccessible matches | Wildcard/regex patterns drop properties with non-public setters/types and emit XBG015/XBG014 if nothing accessible remains |
| Static/generic | Static properties or generic type parameters are rejected (diagnostic) |

### XAML Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.ViewModels">
    <!-- ... -->
    <local:SetStatusTextAction TargetObject="{Binding}" Value="New Status" />
    <!-- ... -->
</UserControl>
```
