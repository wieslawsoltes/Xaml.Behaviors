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

## Platform selection

The platform is resolved per compilation:

| Referenced assemblies | Generated code |
| --- | --- |
| `Avalonia.AvaloniaObject` is available | Avalonia |
| only `Microsoft.UI.Xaml.DependencyObject` is available | WinUI / Uno Platform |
| neither | Avalonia |

Set the `XamlBehaviorsSourceGeneratorPlatform` MSBuild property (`Avalonia` or `WinUI`) to force a platform. The package surfaces it to the generator through `buildTransitive/Xaml.Behaviors.SourceGenerators.props`.

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
| `UseDispatcher` | `Dispatcher.UIThread.Post` / `Invoke` | `DispatcherQueue.TryEnqueue`, using the queue of the thread that created the action/trigger (synchronous invocation for reversible change property actions waits for the UI thread) |
| Event triggers | Subscribes with a handler matching the event delegate | Same; works with `RoutedEventHandler`, `TypedEventHandler<TSender, TResult>`, `EventHandler<T>` and custom delegates |
| `SourceName` lookup | Logical tree name scopes | `FrameworkElement.FindName`, walking `Parent`/`VisualTreeHelper.GetParent` |
| Property triggers | `GetObservable` on a styled/direct property | `RegisterPropertyChangedCallback` for a `DependencyProperty` (static field or static property, e.g. `TextBox.TextProperty`), or `INotifyPropertyChanged` for plain CLR properties |

Generated code only uses public WinUI APIs and the public `Xaml.Interactivity` runtime, so it compiles in any Uno Platform head.

## Limitations

* Property triggers on WinUI need a `DependencyProperty` identifier (a `{Name}Property` static field or static property) or a property declared on a type implementing `INotifyPropertyChanged`; other properties report [XBG037](diagnostics.md#xbg037-property-cannot-be-observed-on-winui).
* A dependency property identifier must be declared on a `DependencyObject` (the observed type); identifiers declared on static helper classes report [XBG038](diagnostics.md#xbg038-dependency-property-owner-is-not-a-dependencyobject).
* `[GenerateTypedMultiDataTrigger]` and `[GenerateTypedInvokeCommandAction]` types must derive from `Xaml.Interactivity.StyledElementTrigger` / `StyledElementAction` ([XBG036](diagnostics.md#xbg036-invalid-winui-base-type)).
* `SourceName` only resolves elements on `FrameworkElement` hosts ([XBG022](diagnostics.md#xbg022-sourcename-not-available) is reported otherwise).
* `LastError` properties are registered with the `object` property type (the CLR accessor stays `Exception?`) so that trimming annotations of `DependencyProperty.Register` do not produce warnings in user projects.
