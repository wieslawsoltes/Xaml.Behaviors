# Known Issues and Differences

This article collects the issues found while porting XAML Behaviors, its tests and its sample applications to Uno
Platform, how they were resolved, and the WinUI differences that affect behaviors. Open issues are tracked on GitHub.

## Tracked issues

| Issue | Platform | Summary | Workaround / possible fix |
|-------|----------|---------|---------------------------|
| [#376](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/376) | Uno | `ChangeAvaloniaPropertyAction` cannot convert string values for properties without a default value (`Border.Background`): WinUI dependency properties do not expose their type, the type is inferred from the default value. | Pass typed values (`{StaticResource BlackBrush}`). Fix: infer the type from Uno's bindable metadata or an explicit value type. |
| [#377](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/377) | Uno | `ClickEventTrigger` with a `Button` source control prevents other `HandledEventsToo` triggers from firing. | Fix: register the handled-events pointer handlers with the WinUI typed delegates. |
| [#378](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/378) | Uno | Data context changes raised before the element loads reach behaviors before their `x:Bind` values are set. | Fix: replay the data context notification after `Loaded`. |
| [#379](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/379) | both | `ContextDragBehavior` only starts a drag when the pressed element shares its `DataContext` (text content of a `Button` has its own). | Use a `TextBlock` as content. Fix: compare the visual ancestry instead of data contexts. |
| [#380](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/380) | both | `SaveFilePickerAction.FileTypeChoicesProperty` is registered with the `OpenFilePickerAction` owner. | Fix: register with `SaveFilePickerAction`. |
| [#381](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/381) | both | Drag event triggers do not fire when a drop handler on the same element handles the event. | Attach the triggers to a parent. Fix: observe handled drag events. |
| [#382](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/382) | Uno | `BindingBehavior`/`BindingTriggerBehavior`/`Condition.Binding` cannot receive a binding written in XAML (WinUI applies bindings, there is no `[AssignBinding]`). | Create the binding in code. Fix: Uno-only source/path properties or value semantics. |
| [#383](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/383) | Uno | `AddItemToItemsControlAction`/`InsertItemToItemsControlAction` cannot create a new item per execution (WinUI templates only create elements, no `ObjectTemplate`). | Fix: an item factory provided by the view model. |
| [#384](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/384) | Uno | `ItemsControl` container triggers miss the containers prepared before they attach. | Fix: report the realized elements when attaching. |
| [#385](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/385) | Avalonia | `ItemsControlContainerEventsBehavior` re-subscribes `PreparingContainer` instead of unsubscribing when detached. | Fix: `-=` in the dispose action. |
| [#386](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/386) | both | `CollectionChangedTrigger`/`CollectionChangedBehavior` subscribe twice when `Collection` is set before attaching. | Fix: subscribe only while attached. |
| [#387](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/387) | both | `ListReorderDragBehavior` ends the drag whenever the placeholder is removed. | Fix: separate placeholder removal from the drag reset. |
| [#388](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/388) | Uno | `Xaml.PropertyGenerator` emits IL2087/IL2111 trimming warnings for generic and `Type` properties. | Fix: annotations or justified suppressions on the generated registrations. |
| [#389](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/389) | samples | The FilesPreview, FluidMoveBehavior and StartBuiltAnimationAction sample pages do not work (on both platforms). | Fix the sample pages. |
| [#390](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/390) | Uno headless | Injected mouse wheel input is not delivered and mouse events carry no key modifiers in the headless session. | Fix: a headless pointer input source in the host. |

## Fixed during the port

| Area | Problem | Fix |
|------|---------|-----|
| `Behavior` (both) | A behavior removed from a loaded element kept receiving events. | `Detach` releases the lifecycle. |
| Routed events (Uno) | A method group passed to WinUI's `RemoveHandler(RoutedEvent, object)` silently removed nothing. | `RemoveRoutedEventHandler`; CS8974 is an error in Uno builds. |
| `AutomationNameBehavior`, `ClearItemsControlAction`, `ItemDragBehavior` (both) | Wrong property access, wrong property owner, items moved twice. | Fixed with tests. |
| Behavior lifecycle (Uno) | Behaviors added from an earlier `Loaded` handler were loaded before the existing ones. | Lifecycle stages are raised as one notification. |
| Property helpers (Uno) | Owner-qualified names (`(TextBox.FontSize)`) were rejected; reverting stacked reversible changes wiped newer ones; typed and reflection changes did not stack. | Fixed in `PropertyHelper.Uno.cs`/`ReversiblePropertyChange`. |
| `MultiDataTriggerBehavior`, `DataTriggerBehavior` (Uno) | `SourceName` never resolved; triggers evaluated before the bindings were applied. | Name scopes are searched upwards; triggers evaluate once initialized. |
| `ContextDragBehavior`, `InlineEditBehavior`, `AutoScrollToBottomBehavior` (Uno) | Un-handled presses consumed by the element; edit lost focus immediately; items assigned after attaching were not observed. | Fixed in the Uno code paths. |
| `PropertyValidationBehavior` (Uno) | A `Property` assigned by `x:Bind` (after attaching) was ignored. | The property is observed again when it changes. |
| `ThemeVariantTrigger` (Uno) | The initial evaluation ran before the `x:Bind` values of its actions were set. | Evaluates once the element is loaded. |
| `BindTagToVisualRootDataContextBehavior` (both) | Never bound the tag when declared in XAML (no visual root yet when attached). | Also binds when attached to the visual tree. |
| ReactiveUI (Uno) | The behaviors were built against ReactiveUI 23 while ReactiveUI.Uno requires 25.1 (`TypeLoadException` for `Interaction`). | Both platforms use ReactiveUI 25.1 (see below). |
| `Xaml.PropertyGenerator` code fix | Orphaned `#pragma warning restore` lines after migrating a registration. | Removed with the registration. |

## ReactiveUI 25 (System.Reactive flavor)

The ReactiveUI behaviors use the System.Reactive flavor of ReactiveUI 25.1 on both platforms: `ReactiveUI.Reactive`
25.1.1 (with System.Reactive 7), `ReactiveUI.Avalonia.Reactive` 12.1.5 and `ReactiveUI.Uno.Reactive` 25.0.0. Commands keep
`System.Reactive.Unit` and the System.Reactive operators and schedulers. The types moved to the flavor's namespaces:

| Type | Namespace |
|------|-----------|
| `ReactiveObject`, `ReactiveCommand`, `RoutingState`, `IScreen`, `IRoutableViewModel`, `RaiseAndSetIfChanged` | `ReactiveUI.Reactive` |
| `Interaction<TInput, TOutput>`, `IViewFor<T>` | `ReactiveUI.Binding.Reactive` |
| `ReactiveUserControl<T>`, `RoutedViewHost`, `UseReactiveUI` | `ReactiveUI.Avalonia.Reactive` / `ReactiveUI.Uno.Reactive` |

`Interaction.Handle` returns a task. Source generators that emit `RaiseAndSetIfChanged` without the namespace (for example
ReactiveGenerator) need `<Using Include="ReactiveUI.Reactive" />` in the project.

**Breaking change:** the `Router`, `ViewModel` and `Interaction` properties of the ReactiveUI behaviors use the types of
`ReactiveUI.Reactive`/`ReactiveUI.Binding.Reactive`. In XAML, reference `RoutedViewHost` from
`clr-namespace:ReactiveUI.Avalonia.Reactive;assembly=ReactiveUI.Avalonia.Reactive`. `ReactiveUI.Avalonia.Reactive` 12.1.5
requires Avalonia 12.1.3.

## WinUI differences

These are differences of the WinUI API, not bugs; `src/Uno/README.md` and the status table of `src/Uno/PORTING.md`
describe how the port maps them.

* **Lifecycle:** WinUI has no logical tree, `Initialized` or `AttachedToVisualTree`; the Avalonia lifecycle is raised
  from `Loaded`/`Unloaded`. Behaviors declared in XAML are attached when the view is created, and `x:Bind` values are
  applied when it loads — evaluate in `OnLoaded` (or later) when an `x:Bind` value is needed.
* **Routed events:** always bubble; `RoutingStrategies.Tunnel` maps to `Preview*` key events or handled events.
  `Button.Click` is not a routed event (use `TappedEvent` for routed triggers). WinUI buttons click on key **up**.
* **Bindings:** no `StringFormat`, `x:Static`, `x:TypeArguments`, `#name`/`$parent[...]`, `OneWayToSource`, or default
  `TwoWay` modes; `x:Bind` defaults to `OneTime`. Generic behaviors are used through closed subclasses.
* **Features without a WinUI counterpart:** style classes, element cursors (only `ProtectedCursor`), adorners,
  Avalonia gesture recognizers (pinch, pull, scroll, touch pad), notification managers, `NumericUpDown`, Screens,
  `Window.BeginMoveDrag`, immediate mode drawing into a `RenderTargetBitmap`, resources changed notifications,
  `ObjectTemplate`, the managed drag and drop. The behaviors built on them are not part of the Uno packages.
* **Controls:** `ListView` instead of `ListBox` (Uno's `ListBox` produces `ContentPresenter` containers), `TabView`,
  `AutoSuggestBox`, `FlipView`, `ContentDialog`, `NumberBox`, `ItemsRepeater` for the container triggers.

## Uno Platform limitations found

Upstream behavior of Uno Platform 6.7 that affects behaviors and samples (worth reporting to Uno Platform):

* `x:Bind` cannot reference named elements inside a `DataTemplate`; use `{Binding ElementName=...}`.
* Visual state setters only reach elements inside the element declaring the states.
* The XAML generator does not escape C# keywords used as names (a visual state named `short` breaks the build) and
  emits `Windows.Foundation.Point` for `RenderTransformOrigin`, which does not compile inside a `*.Windows` namespace.
* The Uno Community Toolkit `DataGrid` does not load with an element (`TextBlock`) column header.
* `InputInjector` applies absolute mouse moves relative to the current position and needs advancing event times
  (`UnoHeadlessSession.Mouse` handles both); keyboard injection raises nothing without a keyboard input source (the
  headless host provides one).
* Composition animations run on the real time clock (`Compositor.GlobalPlaybackRate` is not implemented).
* `UIElement.OpacityTransition`, `QuaternionKeyFrameAnimation` and `Visual.Orientation` with `CenterPoint` are not
  implemented; `DisplayArea` and the screen size are not available on Skia desktop.
