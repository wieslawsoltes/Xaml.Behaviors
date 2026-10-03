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

[Behavior differences: Uno Platform vs Avalonia](../../docfx/articles/uno-platform/behavior-differences.md) lists
every member that behaves differently, the Uno-only API and the features that are not available, with the reasons.
The essentials:

* **Lifecycle**: WinUI has no logical tree, `Initialized` or `AttachedToVisualTree`. Behaviors receive
  `OnInitializedEvent`, `OnAttachedToLogicalTree`, `OnAttachedToVisualTree` and `OnLoaded` (in that order) from
  `FrameworkElement.Loaded`, and the detach notifications in reverse order from `Unloaded`. `x:Bind` values are set
  when the view loads; a data context change raised before that reaches `OnDataContextChangedEvent` once, after
  `OnLoaded`. `OnResourcesChangedEvent` is never raised.
* **Element names**: `[ResolveByName]` does not exist in WinUI XAML; use `{Binding ElementName=...}` or `{x:Bind}`.
* **Bindings as values**: WinUI applies a binding assigned to a property (there is no `[AssignBinding]`).
  `Condition.Binding`, `BindingTriggerBehavior.Binding` and `BindingBehavior.Binding` are `object` properties that
  receive the bound value (like `DataTriggerBehavior.Binding`); a `BindingBase` assigned in code is still applied.
  `BindingBehavior` pushes the value one way to `TargetProperty` and clears it when it leaves the visual tree.

  ```xml
  <icustom:BindingBehavior TargetObject="{x:Bind TargetText}"
                           TargetProperty="{x:Bind mux:TextBlock.TextProperty}"
                           Binding="{x:Bind SourceBox.Text, Mode=OneWay}" />
  ```
* **XAML**: no default `TwoWay` binding modes or value inheritance (write `Mode=TwoWay`), no `StringFormat`,
  `x:Static` or `x:TypeArguments` (use closed generic subclasses), no `XmlnsDefinition` (use `using:` namespaces).
* **Routed events** always bubble: `RoutingStrategies.Tunnel` uses the `Preview*` key events or also receives handled
  events, and `Direct` subscribes like `Bubble`. `GotFocus`/`LostFocus` and `Button.Click` are not routed.
* **Property system**: `SetCurrentValue` and `OverrideMetadata` defaults set local values; temporary values use
  `DependencyPropertyValuePrecedences.Animations`.
* **Style classes** map to visual states (`VisualStateManager`); the class actions are not available.
* **Controls** map to their WinUI counterparts (`ListView`/`Selector`, `TabView`, `AutoSuggestBox`, `FlipView`,
  `ContentDialog`, `ItemsRepeater`, `ElementTheme`).
* **Clipboard and pickers** use the application wide WinUI services (`SystemClipboard`, `SystemStorageProvider`); the
  `Clipboard` and `StorageProvider` properties accept other implementations. The system pickers ignore the start
  folder, title and overwrite prompt.
* **Templates**: instead of Avalonia's `BehaviorCollectionTemplate`, set the attached `i:Interaction.BehaviorsTemplate`
  to a `DataTemplate` whose root is an `i:BehaviorCollectionHost`; every element gets its own collection.
* **New items per execution**: `AddItemToItemsControlAction` and `InsertItemToItemsControlAction` have an
  `ItemFactory` property (`IItemFactory.CreateItem()`) instead of an `ObjectTemplate` item.
