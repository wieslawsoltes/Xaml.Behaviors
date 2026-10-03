# Uno Platform port — porting guide

The Uno Platform (WinUI API) port of Xaml Behaviors shares the Avalonia sources instead of copying them.
This guide describes the layout, the rules and the deterministic workflow used for every project.

## Layout

| Avalonia | Uno Platform |
|----------|--------------|
| `src/Xaml.Behaviors.<Name>/` | `src/Uno/Xaml.Behaviors.<Name>/` (Uno.Sdk project, `Xaml.Behaviors.Uno.<Name>` assembly/package) |
| `tests/Xaml.Behaviors.<Name>.UnitTests/` | `tests/Uno/Xaml.Behaviors.<Name>.UnitTests/` (xUnit v3 + `[UnoHeadlessFact]`) |
| `AvaloniaBehaviors.slnx` | `UnoBehaviors.slnx` |

A Uno project compiles the Avalonia sources of its twin (`<UnoSharedSourceProject>` in the project file,
`src/Uno/Directory.Build.targets`) with the `UNO` preprocessor symbol, minus the files listed in its
`SharedSources.props` (`UnoSharedSourceExcludes`). Files that only exist on Uno live next to the Uno project file,
mirroring the Avalonia folder structure. `SharedSources.props` is also imported by the Avalonia twin so that the
portability analyzers know which files are shared.

## Rules

1. **Share first.** Keep one source file whenever the behavior is the same on both platforms. Use `#if UNO`
   blocks only for real differences, and prefer small platform leaf helpers (see `PropertyHelper`) or partial
   classes (`*.Uno.cs` next to the Uno project) over large `#if` regions.
2. **Namespaces** map `Avalonia.Xaml.<X>` → `Xaml.<X>`:
   ```csharp
   #if UNO
   namespace Xaml.Interactivity;
   #else
   namespace Avalonia.Xaml.Interactivity;
   #endif
   ```
   `using Avalonia.*` directives go into an `#if UNO … #else … #endif` block with the WinUI usings in the Uno part
   (`build/UnoPort/uno_share.py` does both mechanically).
3. **Properties are generated.** Declare `[StyledProperty]`, `[DirectProperty]` or `[AttachedProperty]` partial
   properties (`src/Xaml.PropertyGenerator`). The generator emits Avalonia properties or WinUI dependency
   properties; on WinUI the changes are routed to `OnPropertyChanged(DependencyPropertyChangedEventArgs)` and to
   optional `partial void On<Name>Changed(old, new)` hooks. Hand-written registrations remain only where the
   generator cannot express them (validation/coercion, custom attached accessors) and need `#if UNO`.
4. **Type aliases** (Uno builds only, `src/Uno/Directory.Build.props`) keep the shared code readable:

   | Avalonia name | WinUI type |
   |---------------|------------|
   | `AvaloniaObject` | `DependencyObject` |
   | `AvaloniaProperty` | `DependencyProperty` |
   | `AvaloniaPropertyChangedEventArgs` | `DependencyPropertyChangedEventArgs` |
   | `Control`, `StyledElement`, `Layoutable` | `FrameworkElement` |
   | `Visual`, `InputElement`, `Interactive` | `UIElement` |
   | `MenuItem` | `MenuFlyoutItem` |

   In shared code these names always mean the Avalonia concept. Uno-only files use fully qualified names when they
   need `Microsoft.UI.Xaml.Controls.Control`.
5. **Compat helpers** (`src/Uno/Xaml.Behaviors.Interactivity/Compat`, internal unless noted, visible to the other
   Uno assemblies through `InternalsVisibleTo`): `Dispatcher.UIThread` (`Post`, `Invoke`, `InvokeAsync`,
   `CheckAccess`), routed event helpers, `StyledPropertyMetadata<T>`/`OverrideMetadata`, `DragDrop`, `ILogical`
   (action tree), `AnonymousObserver<T>`, `AvaloniaObjectExtensions.GetObservable<T>`, `GetNewValue`/`GetOldValue`,
   `IsSet`, `SetCurrentValue`, `SetAndRaise`, style classes (visual states), `TopLevel` and the `ResolveByName`
   marker. The shared code uses WinUI's own `DispatcherTimer`. Public compat types (part of the API):
   `RoutingStrategies`, `KeyGesture`, `RoutedEvent<TEventArgs>` and `LogicalTreeAttachmentEventArgs`; public Uno-only
   Interactivity types outside `Compat`: `BehaviorCollectionHost`, `DelegateAddEventHandler<TTarget, THandler>`.
   Extend them when the same Avalonia API is used in several places; otherwise use `#if UNO` locally.
6. **Uno specifics to remember**
   - Classes deriving directly from `DependencyObject` (`AvaloniaObject`) must be `partial` (Uno's
     `DependencyObject` is an interface implemented by a source generator). Such classes have no
     `OnPropertyChanged` virtual: declare `protected virtual void OnPropertyChanged(...)` under `#if UNO`.
   - `GetValue` returns `object`: cast in hand-written getters.
   - WinUI has no logical tree, no `Initialized`, no `AttachedToVisualTree`: `Interaction` raises the Avalonia
     lifecycle (initialized → logical → visual → loaded) from `FrameworkElement.Loaded`, in reverse from
     `Unloaded`. Actions reach their trigger's element through `Action.Host`.
   - Bindings assigned in XAML are applied, not assigned (`[AssignBinding]` has no equivalent): bind the property
     and compare its value.
   - `IValueConverter.Convert` takes a language string instead of a `CultureInfo`.
   - `SetCurrentValue` sets a local value; temporary values use `DependencyPropertyValuePrecedences.Animations`.
   - **Routed events**: keep `element.AddHandler(InputElement.XEvent, Handler, RoutingStrategies)` (the Uno compat
     wraps the handler in the WinUI delegate type; `Tunnel` maps to `Preview*` events or `handledEventsToo`), but
     **remove handlers with `RemoveRoutedEventHandler(...)`**. A method group passed to WinUI's instance
     `UIElement.RemoveHandler(RoutedEvent, object)` binds to it through its natural delegate type and removes
     nothing; Uno builds treat the corresponding warning (CS8974) as an error.
   - Avalonia event argument names are aliased (`PointerPressedEventArgs` → `PointerRoutedEventArgs`,
     `KeyEventArgs` → `KeyRoutedEventArgs`, `TextInputEventArgs` → `CharacterReceivedRoutedEventArgs`, …).
     `GotFocusEvent`, `LostFocusEvent` and `TextInputEvent` are provided as static extension members on
     `UIElement`, `DragDrop.*Event` by a compat `DragDrop` type.
   - `XProperty.OverrideMetadata<T>(new StyledPropertyMetadata<TValue>(value))` works: the default is recorded
     per type and applied as a local value by the base class constructors.
   - Routed events always bubble; `RoutingStrategies.Tunnel` maps to `Preview*` events where WinUI has them (and to
     handled events otherwise, presented to the handler as not handled and restored afterwards); `Direct` alone only delivers the events raised on the element itself
     (`OriginalSource` is the element; `PointerEntered`/`PointerExited` are already raised per element and
     `PointerCaptureLost` is delivered for the captures the element held, see `Compat/DirectRouteFilter.cs`).
7. **Excluding files** is fine when the feature has no WinUI counterpart (Avalonia templates, `TopLevel`
   specific features, notification managers, …). List them in `SharedSources.props` and in the project table
   below with the reason.
8. **Reflection**: do not add new reflection. Porting an existing reflection based feature (for example property
   access by name) with the same `[RequiresUnreferencedCode]` contract is acceptable. Like the Avalonia libraries,
   the packable Uno libraries import `build/TrimmingEnable.targets` (from `src/Uno/Directory.Build.targets`): they
   are marked `IsTrimmable`/`IsAotCompatible` and the IL2xxx/IL3xxx analyzer warnings are errors. Annotate with
   `[DynamicallyAccessedMembers]` or flow the `[RequiresUnreferencedCode]`/`[RequiresDynamicCode]` of the Avalonia
   twin instead of suppressing. The headless test harness projects opt out with `<IsTrimmable>false</IsTrimmable>`.

## Workflow for a project

```bash
# 0. Baseline for the public API check (once per session)
dotnet build AvaloniaBehaviors.slnx -c Release
mkdir -p /tmp/baseline && cp src/Xaml.Behaviors*/bin/Release/net10.0/Xaml.Behaviors*.dll /tmp/baseline/

# 1. Generate the properties (Avalonia build, deterministic code fix). dotnet format loads the analyzers from the
#    Debug output, so build the project in Debug first.
dotnet build src/Xaml.Behaviors.<Name>/Xaml.Behaviors.<Name>.csproj -c Debug
dotnet format analyzers src/Xaml.Behaviors.<Name>/Xaml.Behaviors.<Name>.csproj --diagnostics XPG1001 --severity info

# 2. Share the sources (namespaces, usings, partial, getter casts)
python3 build/UnoPort/uno_share.py src/Xaml.Behaviors.<Name>
```

3. Create `src/Uno/Xaml.Behaviors.<Name>/` (copy the Interactivity project file, set `UnoSharedSourceProject`,
   `AssemblyName`, `PackageId`, `RootNamespace`, `ProjectReference`s to the Uno twins) and `SharedSources.props`.
4. Build the Uno project and port the remaining differences (rules above). Keep Avalonia behavior unchanged.
5. Opt the Avalonia project into the portability analyzers (project specific port map additions go to
   `src/Uno/<Project>/<Project>.xamlport.json`; the analyzers merge all maps) (`<XamlPortEnabled>true</XamlPortEnabled>`, imports of
   `build/XamlPort.props`/`.targets`) and fix every `XPORT` finding — guard the code or, when the Uno port
   provides the API, add it to `build/UnoPort/avalonia-to-uno.xamlport.json`.
6. Verify the Avalonia side: `dotnet build AvaloniaBehaviors.slnx -c Release`, the Avalonia tests, and
   `build/UnoPort/api-compat.sh /tmp/baseline` (no breaking changes).
7. Add Uno tests under `tests/Uno/` and the projects to `UnoBehaviors.slnx`; `dotnet test UnoBehaviors.slnx`.

## Status

[Behavior differences: Uno Platform vs Avalonia](../../docfx/articles/uno-platform/behavior-differences.md) documents
the user-visible differences of every package.

| Project | Uno port | Notes |
|---------|----------|-------|
| Xaml.Behaviors.Interactivity | ✅ | `BehaviorCollectionTemplate` is replaced by `Interaction.BehaviorsTemplate` + `BehaviorCollectionHost` on Uno; `ActionCollectionTemplate`, `ObjectTemplate` and `NotificationTemplate` have no counterpart (`Templates/**`); the C# 14 `Behaviors` extension is Avalonia only. `StyledElementBehavior`/`Trigger`/`Action` derive from `Behavior`/`Trigger`/`Action` (no `StyledElement`). `EventTriggerBehavior` has no `ToolTipOpening`/`ToolTipClosing` events. |
| Xaml.Behaviors.Interactions | ✅ | Clipboard uses a public `IClipboard`/`SystemClipboard` (DataTransfer.Clipboard); pickers use the Avalonia shaped `IStorageProvider`/options types backed by `SystemStorageProvider` (Windows.Storage.Pickers, no arbitrary start folder). Composite actions use the action tree (`Action.Host`). |
| Xaml.Behaviors.Interactions.Events | ✅ | Scroll gesture and IME client events have no WinUI counterpart. |
| Xaml.Behaviors.Interactions.Responsive | ✅ | Style classes map to visual states (`VisualStateManager`): adding a class moves the control to the state of the same name, removing it to `Not{Name}`, `Normal` or `Default`. Bounds come from `ActualOffset` and the actual size. |
| Xaml.Behaviors.Interactions.Draggable | ✅ | `SelectionAdorner` is excluded (no adorner layer); the list reorder placeholder is shown in a non-interactive `Popup`. Pointer capture on press, `TranslateTransform` instead of transform operations, `ChangeView` for scrolling. |
| Xaml.Behaviors.Interactions.DragAndDrop | ✅ | `ManagedDragDrop/**` is excluded (it builds `DragEventArgs` and top-level windows; the WinUI drag is already in-process on Uno Skia/WASM). `DragDropEffects` is `DataPackageOperation`; drop targets that do not set effects accept what the source allows. |
| Xaml.Behaviors.Interactions.DragAndDrop.DataGrid | ✅ | Built against the Uno maintained `Uno.CommunityToolkit.WinUI.UI.Controls.DataGrid`. The row classes map to visual states; the adorner based `Styles.axaml` row indicators are not ported. |
| Xaml.Behaviors.Interactions.Custom | ✅ | Every folder is ported. Excluded (no WinUI counterpart): style class actions, resources changed triggers, `Cursor/**` (only the protected `ProtectedCursor` exists), visual debug adorner, pinch/pull/scroll/touch pad gestures, IME client events, `NumericUpDownValidationBehavior`, `Notifications/**`, `Screen/**`, `CenterWindowBehavior`, `WindowDragMoveBehavior`, the RenderTarget drawing behaviors and `ItemsControlPreparingContainerTrigger`. `ScreenshotAction` renders with WinUI `RenderTargetBitmap`, encodes the PNG with `BitmapEncoder` and saves through the Interactions `IStorageProvider` (`StorageProvider` property, `SystemStorageProvider` by default), so the Uno package references `Xaml.Behaviors.Uno.Interactions`. Controls map to their WinUI counterparts (`AutoSuggestBox`, `FlipView`, `TabView`, `ContentDialog`, `ItemsRepeater`, `Selector`, `ListViewBase`, …). |
| Xaml.Behaviors.Interactions.ReactiveUI | ✅ | Depends on `ReactiveUI.Reactive` 25.1 (the System.Reactive flavor of ReactiveUI, like `ReactiveUI.Uno.Reactive`). |
| Xaml.Behaviors.Interactions.Scripting | ✅ | Same `[RequiresUnreferencedCode]` contract; needs a runtime with dynamic code (not AOT-only targets). |
| Xaml.Behaviors.Animations | ✅ | Avalonia `Animation` becomes a WinUI `Storyboard` (`PlatformAnimation` alias); composition offsets are relative to the layout position on Uno Platform; tilt/orbit rotate around an axis; the selection indicator starts explicit key frame animations (no implicit animations on Uno). |
| Xaml.Behaviors (single assembly) | ✅ | `src/Uno/Xaml.Behaviors` (`Xaml.Behaviors.Uno`) compiles exactly the sources of the Interactivity, Animations, Interactions, Custom, DragAndDrop, Draggable, Events and Responsive Uno projects. |
| Xaml.Behaviors.Avalonia (meta package) | ✅ | `src/Uno/Xaml.Behaviors.All` (`Xaml.Behaviors.Uno.All`) references the same packages. |
| Xaml.Behaviors.SourceGenerators | ✅ | Same generator project; platform strategy (`IXamlPlatform`) with Avalonia and WinUI emitters, override with `XamlBehaviorsSourceGeneratorPlatform`. See `docfx/articles/source-generators/uno-platform.md`. |

## Tooling

| Tool | Purpose |
|------|---------|
| `src/Xaml.PropertyGenerator` | Generates Avalonia/WinUI properties; XPG1001 migrates hand-written registrations. |
| `src/Xaml.PortAnalyzers` | XPORT001–005: partial dependency objects, unguarded platform APIs, unknown `#if` symbols, uncast `GetValue`, invalid port maps. |
| `src/Uno/Xaml.Behaviors.Uno.Headless*` | Headless Uno test host, session harness and `[UnoHeadlessFact]`. |
| `build/UnoPort/uno_share.py` | Mechanical source sharing transforms (idempotent). |
| `build/UnoPort/api-compat.sh` | Strict public API comparison of the Avalonia assemblies against a baseline. |
