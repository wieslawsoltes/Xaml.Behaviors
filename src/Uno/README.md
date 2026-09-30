# XAML Behaviors for Uno Platform

The Uno Platform (WinUI API) port of XAML Behaviors. It is built from the same sources as the Avalonia packages
(see [PORTING.md](PORTING.md) for how the sources are shared) and targets Uno Platform 6.7+ on .NET 10.

## Packages

| Package | Contents |
|---------|----------|
| `Xaml.Behaviors.Uno.Interactivity` | `Behavior`, `Trigger`, `Action`, `Interaction`, collections, conditions, event registry |
| `Xaml.Behaviors.Uno.Interactions` | Core triggers and actions (`EventTriggerBehavior`, `InvokeCommandAction`, `CallMethodAction`, `ChangePropertyAction`, data triggers, timers, debounce/throttle, clipboard, file/folder pickers, file system and network) |
| `Xaml.Behaviors.Uno.Interactions.Events` | Input and focus event triggers and behaviors |
| `Xaml.Behaviors.Uno.Interactions.Custom` | The large collection of custom behaviors, triggers, actions and converters |
| `Xaml.Behaviors.Uno.Interactions.Responsive` | Adaptive and aspect ratio behaviors (style classes map to visual states) |
| `Xaml.Behaviors.Uno.Interactions.Draggable` | Canvas, grid, item and list reorder drag behaviors |
| `Xaml.Behaviors.Uno.Interactions.DragAndDrop` | Context, typed, text and file drag and drop behaviors |
| `Xaml.Behaviors.Uno.Interactions.DragAndDrop.DataGrid` | DataGrid row drag and drop (Uno Community Toolkit DataGrid) |
| `Xaml.Behaviors.Uno.Animations` | Storyboard, composition and transition helpers (no behaviors dependency) |
| `Xaml.Behaviors.Uno.Interactions.ReactiveUI` | ReactiveUI navigation and interaction behaviors |
| `Xaml.Behaviors.Uno.Interactions.Scripting` | C# scripting actions (needs dynamic code) |
| `Xaml.Behaviors.Uno` | Single assembly with Interactivity, Animations, Interactions, Custom, DragAndDrop, Draggable, Events and Responsive |
| `Xaml.Behaviors.Uno.All` | Meta package referencing the same packages individually |

See the status table in [PORTING.md](PORTING.md#status) for the features that have no WinUI counterpart.

Supporting packages that are useful beyond this library:

| Package | Purpose |
|---------|---------|
| `Xaml.Behaviors.Uno.Headless`, `Xaml.Behaviors.Uno.Headless.XUnit`, `Xaml.Behaviors.Uno.Headless.Host` | Headless Uno Platform UI tests with xUnit v3 (`[UnoHeadlessFact]`), see [the harness README](Xaml.Behaviors.Uno.Headless/README.md). |
| `Xaml.PropertyGenerator` | Generates Avalonia and WinUI/Uno properties from one declaration; migrates hand written registrations. |
| `Xaml.PortAnalyzers` | Portability analyzers for Avalonia → Uno Platform ports. |

## Usage

```xml
<Page xmlns:i="using:Xaml.Interactivity"
      xmlns:ic="using:Xaml.Interactions.Core"
      xmlns:ie="using:Xaml.Interactions.Events">
  <Button Content="Save">
    <i:Interaction.Behaviors>
      <ic:EventTriggerBehavior EventName="Click">
        <ic:InvokeCommandAction Command="{x:Bind ViewModel.SaveCommand}" />
      </ic:EventTriggerBehavior>
    </i:Interaction.Behaviors>
  </Button>
</Page>
```

Namespaces follow the Avalonia ones without the `Avalonia.` prefix: `Avalonia.Xaml.Interactivity` →
`Xaml.Interactivity`, `Avalonia.Xaml.Interactions.Core` → `Xaml.Interactions.Core`, and so on.

## Differences from the Avalonia packages

* **Lifecycle**: WinUI has no logical tree, `Initialized` or `AttachedToVisualTree` notifications. Behaviors receive
  `OnInitializedEvent`, `OnAttachedToLogicalTree`, `OnAttachedToVisualTree` and `OnLoaded` (in that order) from
  `FrameworkElement.Loaded`, and the detach notifications in reverse order from `Unloaded`.
* **Element names**: `[ResolveByName]` does not exist in WinUI XAML; use `{Binding ElementName=...}` or `{x:Bind}`.
* **Bindings as values**: WinUI applies a binding assigned to a property; `Condition.Binding` and
  `DataTriggerBehavior.Binding` therefore compare the bound value.
* **Routed events**: WinUI routed events always bubble. `RoutingStrategies.Tunnel` uses the `Preview*` event when one
  exists (key events) and otherwise also receives handled events.
* **Default binding modes and value inheritance** are Avalonia features; on WinUI specify `Mode=TwoWay` explicitly.
* **Clipboard and pickers** use the application wide WinUI services (`SystemClipboard`, `SystemStorageProvider`).
  The `Clipboard` and `StorageProvider` properties accept other implementations, for example in tests. WinUI pickers
  cannot open an arbitrary start folder.
* **Templates**: instead of Avalonia's `BehaviorCollectionTemplate`, set the attached `i:Interaction.BehaviorsTemplate`
  (for example from a style setter) to a `DataTemplate` whose root is an `i:BehaviorCollectionHost` containing the
  behaviors; every element gets its own collection. The other Avalonia only features are listed in PORTING.md.
