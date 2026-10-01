# Known Issues and Differences

This article collects the issues found while porting XAML Behaviors, its tests and its sample applications to Uno
Platform, how they were resolved, and the WinUI differences that affect behaviors. Open issues are tracked on GitHub.

## Tracked issues

All issues found while porting the libraries, tests and samples
([#376](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/376)–[#390](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/390)) and while documenting the
[behavior differences](behavior-differences.md) ([#391](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/391)–[#399](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/399)) are fixed; see the
table below. Open issues:

| Issue | Area | Problem |
|-------|------|---------|
| [#400](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/400) | Routed event behaviors (Avalonia) | A routing strategy that does not match the event's routes never fires: `ExecuteCommandOnPointerEntered/Exited/CaptureLostBehavior` (default `Bubble` on Direct events), and an explicit `Direct` on a bubbling event outside the `RoutedEventTriggerBase` triggers. |

Report new issues on [GitHub](https://github.com/wieslawsoltes/Xaml.Behaviors/issues).

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
| `SaveFilePickerAction` (both, [#380](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/380)) | `FileTypeChoicesProperty` was registered with the `OpenFilePickerAction` owner (bindings by name missed its changes). | Registered with `SaveFilePickerAction`. |
| `ItemsControlContainerEventsBehavior` (Avalonia, [#385](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/385)) | Detaching re-subscribed `PreparingContainer` instead of unsubscribing (the behavior kept receiving it, twice after re-attaching). | Unsubscribes in the dispose action. |
| `CollectionChangedTrigger`, `CollectionChangedBehavior` (both, [#386](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/386)) | A `Collection` set before attaching was observed twice (actions ran twice per change) and still observed after detaching. | Observe the collection only while attached, through one tracked subscription. |
| `ListReorderDragBehavior` (both, [#387](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/387)) | Removing the placeholder also ended the drag, so a move without a target or to another target stopped the placeholder (it stayed on the first target or never appeared). | Removing the placeholder is separate from the drag reset (release/capture lost only). |
| FilesPreview, FluidMoveBehavior, StartBuiltAnimationAction samples (both, [#389](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/389)) | `AddPreviewFilesAction` ignored a list without `ItemsSource`; `FluidMoveBehavior` did nothing on an `ItemsControl` and lost the positions of recreated containers; the built animation targeted the `Text` of the `Button`. | `AddPreviewFilesAction` fills `Items` without an `ItemsSource`; `FluidMoveBehavior` animates the items panel of an `ItemsControl` and tracks containers by their item; the page runs the action on the `TextBlock` (`ObservableStreamBehavior` + command). |
| `ClickEventTrigger` (Uno, [#377](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/377)) | A trigger using a `Button` as source control never clicked: the button releases the pointer capture in its own release handler, before the routed handler of the trigger, which cancelled the press. | A capture lost by the release itself keeps the press. |
| Behavior lifecycle (Uno, [#378](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/378)) | Data context changes raised before the element loaded reached the behaviors before their `x:Bind` values were set. | A change raised before the behaviors are loaded is delivered once, after the `Loaded` lifecycle. |
| Drag event triggers (both, [#381](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/381)) | `DragEnterEventTrigger`, `DragOverEventTrigger`, `DropEventTrigger` and `DragLeaveEventTrigger` did not fire when a drop handler on the same element handled the event. | The triggers also receive handled events (no public API change). |
| `ItemsControl` container triggers (Uno, [#384](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/384)) | The triggers subscribed to the `ItemsRepeater` when it loaded, after the first layout pass prepared the initial elements. | The triggers subscribe as soon as they are attached. |
| `ContextDragBehavior`, `ContextDragWithDirectionBehavior` (both, [#379](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/379)) | A drag only started when the pressed element shared the drag source's `DataContext`, so pressing the text of a `Button` or other templated content did nothing. | A press starts the drag when it comes from the drag source or one of its visual descendants that is not inside a nested drag source. |
| `BindingBehavior`, `BindingTriggerBehavior`, `Condition` (Uno, [#382](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/382)) | `Binding` could not receive a binding written in XAML: WinUI applies the binding instead of assigning it (there is no `[AssignBinding]`), so the samples created binding objects in code. | `Binding` is an `object` property on Uno Platform with value semantics: the behaviors use the bound value (`BindingBehavior` pushes it to the target property), a `BindingBase` assigned in code is still applied. |
| `AddItemToItemsControlAction`, `InsertItemToItemsControlAction` (Uno, [#383](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/383)) | The actions could not create a new item per execution (WinUI templates only create UI elements, there is no `ObjectTemplate`): the samples added the same item on every click. | Uno-only `ItemFactory` property (`IItemFactory.CreateItem()`), used instead of `Item`; the samples (including the ObjectTemplate page) declare a view model item factory. |
| `ChangeAvaloniaPropertyAction` (Uno, [#376](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/376)) | String values were not converted for properties without a default value (`Border.Background`): WinUI dependency properties do not expose their type (Uno's bindable metadata is looked up by property name, which a `DependencyProperty` does not expose either). | The type is also inferred from the current value (strings only, unconvertible strings are assigned as is); the Uno-only `TargetPropertyType` sets it explicitly. |
| Headless session (Uno, [#390](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/390)) | Injected mouse wheel input was not delivered (Uno Platform reads the wheel rotation from `DeltaY`, not `MouseData`) and mouse events carried no keyboard modifiers (`InputInjector.InjectMouseInput` raises none). | The host injects mouse input with modifiers and the WinUI wheel data (`HeadlessHost.InjectMouseInput`); `UnoHeadlessMouse` passes the modifiers held on `UnoHeadlessKeyboard` and the ones given to its members. |
| `Xaml.PropertyGenerator` WinUI output ([#388](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/388)) | IL2087/IL2111 trimming warnings for properties typed with a generic parameter (validation rules) or `System.Type` (`TypedDragBehaviorBase.DataType`). | Generic parameters get the `[DynamicallyAccessedMembers]` annotation of `DependencyProperty.Register`; `Type` properties are registered through a helper with a justified IL2111 suppression. |
| Routed events (Uno, [#391](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/391)) | `RoutingStrategies.Direct` behaved like `Bubble`: Direct triggers also fired for events raised by child elements. | A Direct-only subscription runs the handler only for events whose `OriginalSource` is the element; `PointerEntered`/`PointerExited` are raised per element by Uno and are not filtered; `PointerCaptureLost` is delivered for the captures the element held. |
| `ExecuteCommandBehaviorBase` (Avalonia, [#392](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/392)) | `FocusControlProperty` was registered with the name `CommandParameter`. | Registered as `FocusControl`. |
| `HideAttachedFlyoutBehavior` (Avalonia, [#393](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/393)) | `IsFlyoutOpenProperty` was registered with the `ButtonHideFlyoutBehavior` owner. | Registered with `HideAttachedFlyoutBehavior`. |
| `TextInputTrigger` (Avalonia, [#394](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/394)) | `TextProperty` was registered with the `KeyDownTrigger` owner. | Registered with `TextInputTrigger`. |
| Pointer triggers and command behaviors (Uno, [#395](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/395)) | An emulated `Tunnel` subscription received handled events: triggers set `Handled = false` on them and `ExecuteCommandOnPointer*Behavior` skipped them. | Triggers only set `Handled` when `MarkAsHandled` is true; the emulated tunnel handler sees a handled event as not handled and the flag is restored afterwards. |
| `PropertyValidationBehavior` (Uno, [#396](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/396)) | Custom validation rules could not trigger revalidation when their properties changed. | `IValidationRuleChanged` is public: custom rules raise `Changed` to revalidate. |
| `ScreenshotAction` (Uno, [#397](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/397)) | Not ported. | Ported with WinUI `RenderTargetBitmap`, `BitmapEncoder` and the Interactions storage provider (new `StorageProvider` property on both platforms). |
| Packaging (Uno, [#398](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/398)) | The Uno packages were not marked trimmable or AOT-compatible. | `build/TrimmingEnable.targets` is imported for the packable Uno libraries; trim/AOT warnings are errors. |
| `KeyTrigger`, `KeyDownTrigger`, `KeyUpTrigger` (Avalonia, [#399](https://github.com/wieslawsoltes/Xaml.Behaviors/issues/399)) | Never fired with the default `Direct` routing: Avalonia only invokes Direct-only handlers for direct events. | A Direct-only subscription of a `RoutedEventTriggerBase` trigger to a tunneling/bubbling event uses that route, filtered by `e.Source == element`. |

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

These are differences of the WinUI API, not bugs. [Behavior Differences](behavior-differences.md) lists every
affected behavior, trigger and action, the Uno-only API and the features that are not available; the status table of
`src/Uno/PORTING.md` describes how the port maps them.

* **Lifecycle:** WinUI has no logical tree, `Initialized` or `AttachedToVisualTree`; the Avalonia lifecycle is raised
  from `Loaded`/`Unloaded`. Behaviors declared in XAML are attached when the view is created, and `x:Bind` values are
  applied when it loads — evaluate in `OnLoaded` (or later) when an `x:Bind` value is needed. Data context changes
  raised before the element loads are delivered once, after `OnLoaded`.
* **Routed events:** always bubble; `RoutingStrategies.Tunnel` maps to `Preview*` key events or handled events, and
  `Direct` subscribes like `Bubble` (events of descendants are received too).
  `Button.Click` is not a routed event (use `TappedEvent` for routed triggers). WinUI buttons click on key **up**.
* **Bindings:** no `StringFormat`, `x:Static`, `x:TypeArguments`, `#name`/`$parent[...]`, `OneWayToSource`, or default
  `TwoWay` modes; `x:Bind` defaults to `OneTime`. Generic behaviors are used through closed subclasses.
* **Features without a WinUI counterpart:** element cursors (only `ProtectedCursor`), Avalonia gesture recognizers
  (pinch, pull, scroll, touch pad), IME client events, notification managers, `NumericUpDown`, Screens,
  `Window.BeginMoveDrag`, immediate mode drawing into a `RenderTargetBitmap`, resources changed notifications, an event
  before an `ItemsRepeater` prepares a container, `ObjectTemplate` (the item actions use an `ItemFactory` instead) and
  the managed drag and drop. The behaviors built on them are not part of the Uno packages.
* **Style classes and adorners:** the style class actions (`AddClassAction`, `RemoveClassAction`, `ToggleClassAction`),
  `VisualDebugBehavior` and `SelectionAdorner` are not available. The other behaviors that use style classes
  (Responsive, the Draggable `:dragging` class, the tree view and data grid drop handlers) change visual states of the
  same name instead, and `ListReorderDragBehavior` shows its placeholder in a `Popup`.
* **Controls:** `ListView` instead of `ListBox` (Uno's `ListBox` produces `ContentPresenter` containers; the
  select-all and scroll-to-item behaviors require a `ListViewBase`), `TabView`, `AutoSuggestBox`, `FlipView`,
  `ContentDialog` (dialog triggers and `ShowDialogAction`), `NumberBox`, `ItemsRepeater` for the container triggers.

## Uno Platform limitations found

Upstream behavior of Uno Platform 6.7 that affects behaviors and samples (worth reporting to Uno Platform):

* `x:Bind` cannot reference named elements inside a `DataTemplate`; use `{Binding ElementName=...}`.
* Visual state setters only reach elements inside the element declaring the states.
* The XAML generator does not escape C# keywords used as names (a visual state named `short` breaks the build) and
  emits `Windows.Foundation.Point` for `RenderTransformOrigin`, which does not compile inside a `*.Windows` namespace.
* The Uno Community Toolkit `DataGrid` does not load with an element (`TextBlock`) column header.
* `InputInjector` applies absolute mouse moves relative to the current position and needs advancing event times
  (`UnoHeadlessSession.Mouse` handles both), reads the wheel rotation from `DeltaY`/`DeltaX` instead of `MouseData` and
  injects mouse input without keyboard modifiers (the headless host injects both); keyboard injection raises nothing
  without a keyboard input source (the headless host provides one).
* Composition animations run on the real time clock (`Compositor.GlobalPlaybackRate` is not implemented).
* `UIElement.OpacityTransition`, `QuaternionKeyFrameAnimation` and `Visual.Orientation` with `CenterPoint` are not
  implemented; `DisplayArea` and the screen size are not available on Skia desktop.
