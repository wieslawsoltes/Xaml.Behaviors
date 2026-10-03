# Uno Platform (WinUI) Support

`Xaml.Behaviors.SourceGenerators` emits code for both **Avalonia** and **Uno Platform / WinUI**. The same package and the same attributes are used on both frameworks; only the generated code differs.

## Setup

1. Reference `Xaml.Behaviors.Uno.Interactivity` (namespace `Xaml.Interactivity`), the Uno Platform counterpart of `Xaml.Behaviors.Interactivity`.
2. Reference the `Xaml.Behaviors.SourceGenerators` package.
3. Annotate members with the usual attributes (`[GenerateTypedAction]`, `[GenerateTypedTrigger]`, ...).

```csharp
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateTypedTrigger(typeof(Microsoft.UI.Xaml.Controls.Button), "Click")]
[assembly: GeneratePropertyTrigger(typeof(Microsoft.UI.Xaml.Controls.TextBox), "TextProperty")]

public partial class MainViewModel
{
    [GenerateTypedAction]
    public void Submit(string text) { /* ... */ }
}
```

```xml
<Button Content="Submit"
        xmlns:i="using:Xaml.Interactivity"
        xmlns:p="using:Microsoft.UI.Xaml.Controls.Primitives"
        xmlns:vm="using:MyApp.ViewModels">
  <i:Interaction.Behaviors>
    <p:ButtonBaseClickTrigger>
      <vm:SubmitAction TargetObject="{Binding}" Text="{Binding Query}" />
    </p:ButtonBaseClickTrigger>
  </i:Interaction.Behaviors>
</Button>
```

Classes generated for framework types (assembly attributes) are placed in the namespace of the type that declares the member, e.g. `Button.Click` is declared on `ButtonBase`, so the trigger is `Microsoft.UI.Xaml.Controls.Primitives.ButtonBaseClickTrigger`.

> [!IMPORTANT]
> The Uno XAML generator is itself a source generator, and source generators do not see each other's output. XAML in the assembly that runs `Xaml.Behaviors.SourceGenerators` cannot reference the generated types (`UXAML0001`, the type could not be found). Generate the actions and triggers in a separate library (for example the view model library) and reference it from the application, as the `SourceGeneratorSample.Core` project of the Uno samples does. The XAML above assumes that `SubmitAction` comes from such a library.

## Platform selection

The platform is resolved per compilation:

| Referenced assemblies | Generated code |
| --- | --- |
| `Avalonia.AvaloniaObject` is available | Avalonia |
| only `Microsoft.UI.Xaml.DependencyObject` is available | WinUI / Uno Platform |
| neither | Avalonia |

Set the `XamlBehaviorsSourceGeneratorPlatform` MSBuild property (`Avalonia`, or `WinUI`/`Uno`) to force a platform; it wins over the detection. The package surfaces it to the generator through `buildTransitive/Xaml.Behaviors.SourceGenerators.props`.

```xml
<PropertyGroup>
  <XamlBehaviorsSourceGeneratorPlatform>WinUI</XamlBehaviorsSourceGeneratorPlatform>
</PropertyGroup>
```

## What is generated on WinUI

| Concern | Avalonia | WinUI / Uno Platform |
| --- | --- | --- |
| Base classes | `Avalonia.Xaml.Interactivity.StyledElementAction` / `StyledElementTrigger` | `Xaml.Interactivity.StyledElementAction` / `StyledElementTrigger` |
| Properties | `StyledProperty<T>` registered with `AvaloniaProperty.Register` | `DependencyProperty.Register` with a typed CLR accessor; changes are routed to `OnPropertyChanged(DependencyPropertyChangedEventArgs)` |
| `UseDispatcher` | `Dispatcher.UIThread.Post` / `Invoke` | `DispatcherQueue.TryEnqueue`, using the queue of the thread that created the action/trigger, then `DependencyObject.DispatcherQueue`, then the queue of the current thread (synchronous invocation for reversible change property actions waits for the UI thread). Without a queue, or when enqueueing fails, the work runs inline. |
| Event triggers | Subscribes with a handler matching the event delegate | Same; works with `RoutedEventHandler`, `TypedEventHandler<TSender, TResult>`, `EventHandler<T>` and custom delegates |
| `SourceName` lookup | Logical tree name scopes | `FrameworkElement.FindName`, walking `Parent`/`VisualTreeHelper.GetParent` |
| Property triggers | `GetObservable` on a styled/direct property | `RegisterPropertyChangedCallback` for a `DependencyProperty` (static field or static property, e.g. `TextBox.TextProperty`), or `INotifyPropertyChanged` for plain CLR properties (evaluated on every `PropertyChanged` for the property name or an empty name, even when the value did not change) |
| Reversible change property actions | Animation priority overlay; `Revert` restores the latest source value | The value is set as a local value through the typed setter (it replaces a binding); `Revert` restores the captured value |
| `IsSet` checks (event command `Parameter`, async/observable trigger overrides) | Any non-default value source | Local values only (including bindings), not style setters |

Generated code only uses public WinUI APIs and the public `Xaml.Interactivity` runtime, so it compiles in any Uno Platform head.

## Limitations

* Property triggers on WinUI need a `DependencyProperty` identifier (a `{Name}Property` static field or static property) or a property declared on a type implementing `INotifyPropertyChanged`; other properties report [XBG037](diagnostics.md#xbg037-property-cannot-be-observed-on-winui).
* A dependency property identifier must be declared on a `DependencyObject` (the observed type); identifiers declared on static helper classes report [XBG038](diagnostics.md#xbg038-dependency-property-owner-is-not-a-dependencyobject).
* `[GenerateTypedMultiDataTrigger]` and `[GenerateTypedInvokeCommandAction]` types must derive from `Xaml.Interactivity.StyledElementTrigger` / `StyledElementAction` ([XBG036](diagnostics.md#xbg036-invalid-winui-base-type)).
* `SourceName` only resolves elements on `FrameworkElement` hosts ([XBG022](diagnostics.md#xbg022-sourcename-not-available) is reported otherwise).
* `[GeneratePropertyTrigger]` on an instance field, or on a static member that is neither an Avalonia property nor a `DependencyProperty`, still reports [XBG019](diagnostics.md#xbg019-invalid-avalonia-property).
* When the generator cannot find the CLR type of a dependency property (no CLR property and no `Get{Name}` accessor), the trigger's `Value` is typed `object?`.
* `LastError` properties are registered with the `object` property type (the CLR accessor stays `Exception?`) so that trimming annotations of `DependencyProperty.Register` do not produce warnings in user projects.

See [Behavior differences: Uno Platform vs Avalonia](../uno-platform/behavior-differences.md#sourcegenerators) for the differences of the runtime packages.
