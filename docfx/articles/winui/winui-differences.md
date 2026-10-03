# Differences from Uno Platform and Tracked Issues

The WinUI 3 port compiles exactly the sources of the Uno Platform port (see [Overview](index.md)). Uno Platform
implements the WinUI API, but native WinUI (Windows App SDK) differs in places. This article tracks those differences,
how the WinUI port handles them, and the open issues of the port. For the differences between Uno Platform and
Avalonia see [Behavior Differences](../uno-platform/behavior-differences.md).

All findings come from building and running the WinUI libraries, tests and samples on Windows 11 (ARM64) with the
Windows App SDK 2.5.

## Status

| Area | Status |
|------|--------|
| Libraries (`src/WinUI`, 13 packages) | Done: build on Windows (0 errors, no compiler, CsWinRT or trimming warnings). |
| Test harness (`Xaml.Behaviors.WinUI.Testing`) | Done. |
| Test projects (`tests/WinUI`, 16 projects) | Done: 1,020 tests, none failing, 10 skipped (see [Test status](#test-status)). |
| Samples (`samples/WinUI`, 3 applications) | Build and start on Windows. The Behaviors sample shows its pages; the pages that set a `DependencyProperty` typed property in XAML do not load yet ([W20](#w20-dependency-property-types)). |
| CI | A Windows job builds the solution, runs the tests (informative until they are stable on the hosted runners) and packs the packages. |

### Test status

Windows 11 ARM64 (Parallels), `build/WinUIPort/run-tests.ps1`:

| Test project | Tests | Failed | Skipped |
|--------------|------:|-------:|---------|
| Animations | 95 | 0 | 5: composition animations sampled while they run ([W5](#w5-composition-animations-run-in-the-compositor)) |
| Interactions.Custom.Animations | 22 | 0 | |
| Interactions.Custom.Controls | 56 | 0 | |
| Interactions.Custom.General | 110 | 0 | |
| Interactions.DragAndDrop | 23 | 0 | 2: drop handler over tree view items ([W12](#w12-drag-and-drop-runs-a-modal-loop)) |
| Interactions.DragAndDrop.DataGrid | 2 | 0 | |
| Interactions.Draggable | 13 | 0 | |
| Interactions.Events | 4 | 0 | |
| Interactions.ReactiveUI | 8 | 0 | |
| Interactions.Responsive | 7 | 0 | |
| Interactions.Scripting | 5 | 0 | |
| Interactions | 207 | 0 | 2: `FluidMoveBehavior` in a `Canvas` ([W25](#w25-a-canvas-moves-its-children-without-a-layout-pass)), local tab navigation ([W22](#w22-element-tree-events-and-input-timing)) |
| Interactivity | 156 | 0 | |
| SourceGenerators | 247 | 0 | |
| Xaml.Behaviors (single assembly) | 10 | 0 | |
| WinUI test harness | 55 | 0 | 1 (also skipped on Uno Platform) |

The tests inject real input: a window of another application that comes to the front while they run makes input
tests fail (see [W11](#w11-input-is-real-operating-system-input)). On the shared validation machine an occasional
input test of the harness project fails for that reason (a different one in each run, none in most runs).

## Differences

Each difference has the platform behavior, how the port handles it, and its status.

### W1. No window enumeration

**Uno Platform:** `Uno.UI.ApplicationHelper.Windows` lists the open windows; the behaviors find the window of an element
through it (Avalonia `TopLevel.GetTopLevel`).
**WinUI:** there is no API to enumerate the windows of an application.
**Port:** the WinUI-only public `Xaml.Interactivity.WindowTracker`: applications call `WindowTracker.Track(window)` for
each window (it is untracked when closed). The test harness and the samples track their windows. **Done.**

### W2. No main view dispatcher

**Uno Platform:** `CoreApplication.MainView.Dispatcher` is the UI thread dispatcher (the dispatcher compat
`Dispatcher.UIThread` uses it).
**WinUI:** `CoreApplication.MainView` is not available in desktop applications ("Could not create a new view because
the main window has not yet been created").
**Port:** the dispatcher compat uses the `DispatcherQueue` of the UI thread, captured when a behavior is attached or a
window is tracked. **Done.**

### W3. No generic `DependencyObjectCollection<T>`

**Uno Platform:** `DependencyObjectCollection<T>` (Uno only) types the setter and condition collections
(`AdaptiveBehavior.Setters`, `ConditionCollection`, ...).
**WinUI:** only the non-generic `DependencyObjectCollection` exists, and the WinUI XAML compiler rejects a collection
with more than one `Add` method or `ICollection<T>` implementation.
**Port:** on WinUI `ConditionCollection` derives from `DependencyObjectCollection`, and the `Setters` properties use the
WinUI-only `AdaptiveClassSetterCollection`, `AspectRatioClassSetterCollection` and `SplitViewStateSetterCollection`
(`DependencyObjectCollection` with a typed enumerator; items are added through the untyped collection). The shared
code takes `IEnumerable<T>` where it only enumerates. **Done.**

### W4. No value precedences

**Uno Platform:** `SetValue(property, value, DependencyPropertyValuePrecedences.Animations)` sets a temporary value that
overrides the local value or binding without replacing it (property helpers, `CommandCanExecuteIsEnabledBinder`).
**WinUI:** value precedences are not public.
**Port:** `AnimationValueCompat` emulates them on WinUI: the first temporary value saves the local value or binding,
clearing restores it. A local value set while a temporary value is effective is lost when it is cleared. **Done.**

### W5. Composition animations run in the compositor

**Uno Platform:** `Visual.Offset` of an element visual is relative to its layout position, `Translation` needs no
opt-in, and reading a property during an animation returns the animated value.
**WinUI:** layout owns `Visual.Offset` (it is the arranged position); `Translation` must be enabled per element
(`ElementCompositionPreview.SetIsTranslationEnabled`); animations run in the compositor, so the UI thread reads the last
value set, not the animated value.
**Port:** the behaviors move elements with `Translation` on WinUI (enabled once, when they first get the element visual:
enabling it again resets it); the tests read the offset with `GetLayoutRelativeOffset`. The tests that sample an
animation while it runs (sliding and selection indicator animations) are skipped on WinUI. **Done.**

### W6. `RenderTargetBitmap` renders at the rasterization scale

**Uno Platform:** `RenderAsync(element, width, height)` renders that many pixels.
**WinUI:** the size is logical and is rendered at the rasterization scale of the element (twice the pixels at 200 %).
**Port:** `ScreenshotAction` requests the size that renders to the element size in device independent pixels.
**Done, to be verified.**

### W7. Only framework elements have a data context

**Uno Platform:** every `DependencyObject` has a `DataContext` (Uno extension), so behaviors, actions, conditions and
setters can be bound in code with a data context of their own.
**WinUI:** only `FrameworkElement` has a `DataContext`; the items of a `DependencyObjectCollection` use the inheritance
context of the collection owner.
**Port:** tests that set the data context of a behavior bind with an explicit `Source` on WinUI. **In progress.**

### W8. `DependencyObject` is a class

**Uno Platform:** `DependencyObject` is an interface (`where T : class, DependencyObject` is valid).
**WinUI:** it is a class. **Port:** the affected test helpers use `where T : DependencyObject`. **Done.**

### W9. `Panel` cannot be created

**Uno Platform:** `new Panel()` works. **WinUI:** `Panel` has a protected constructor.
**Port:** the shared tests use `Grid` on WinUI. **Done.**

### W10. XAML compiler

The WinUI XAML compiler is stricter than the Uno XAML generator:

| Uno Platform | WinUI | Port |
|--------------|-------|------|
| Setter-only static attached properties (`SetBounce(Control, double)`) | The attached property type comes from the getter (`Void` without one) | The write-only animation attached properties have getters on Uno and WinUI (they return `NaN`). **Done.** |
| Two-way `x:Bind` to a property with a private setter | Not allowed (and there is no `OneWayToSource`) | The behaviors generator gives the output properties of its WinUI output (`IsExecuting`, `LastError`, ...) a public setter. **Done.** |
| `WrapPanel` (Uno only control) | No wrap panel | The samples use their own `WrapPanel`. **Done.** |
| Pages loaded from linked XAML files | Supported (same relative paths, `ms-appx:///` URIs unchanged) | **Done.** |
| `init` accessors on properties set from XAML (`MainWindow.ViewModel`) | The generated XAML type information sets them after construction | The sample property is settable. **Done.** |
| `x:Bind` of a nullable value to a non-nullable property (`CheckBox.IsChecked` to a `bool` property) | Not allowed (WMC1121) | The samples bind through a function (`SampleCheck.IsTrue`). **Done.** |

### W11. Input is real operating system input

**Uno Platform:** the headless host injects input into the Uno input pipeline, and the tests could create key event
arguments (test-only reflection).
**WinUI:** there is no headless host, routed events cannot be raised from code and event arguments cannot be created.
**Port:** the WinUI test harness injects real input (`SendInput`) into its window, which is always on top, brought to the
foreground and checked to be under the cursor; the test helpers use it on WinUI (focus the element, then type).
The tests need an interactive desktop, and the mouse and keyboard must not be used while they run: any other window
that comes to the front (another test run on the same machine, a console window) takes the input. The idle wait of
the session (`WaitForIdleAsync`) also waits for the layout and a rendered frame, because WinUI raises `SizeChanged`,
`Loaded` and the focus events with its frames, not with the queued work. Showing new content releases the mouse
buttons a test left pressed and closes the popups it left open (the operating system keeps both, unlike the Uno
headless host). **Done** (harness and helpers).

### W12. Drag and drop runs a modal loop

**Uno Platform:** drag and drop runs in-process with the injected input.
**WinUI:** `StartDragAsync` runs the OLE drag and drop modal loop on the UI thread, so input injected synchronously
from the UI thread deadlocks (the test waits inside the loop for a release it would inject next).
**Port:** the harness has asynchronous mouse members (`MoveToAsync`, `DownAsync`, `UpAsync`) that inject from a
background thread and let the UI thread (or the modal loop) process the input; the drag and drop test helpers use them.
The test runner stops a test project after a timeout. **In progress.**

The `OriginalSource` of a native WinUI drag event is the drop target (the element that allows the drop), not the
element under the pointer. The drop handlers that look for the item under the pointer (`BaseDataGridDropHandler`,
`BaseTreeViewDropHandler`) get the element under the pointer in the element that receives the event. **Done.**
A handler of an application that reads `e.OriginalSource` (for example to get the data context of the row under the
pointer) gets the drop target on WinUI: find the element under the pointer instead
(`VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), target)`). **Documented.**

WinUI does not route a handled drag event to the ancestors, and a native `TreeViewItem` handles the drag events over
it: a drop handler attached to a `TreeView` (`BaseTreeViewDropHandler`) is called over the empty area of the tree
view but not over its items. Two tests are skipped. **Open (platform).**

The data of a drag is also read asynchronously on WinUI (`DataPackageView.GetTextAsync`, `GetStorageItemsAsync`
complete later; on Uno Platform they are complete for a drag inside the application). The drop behaviors read the data
synchronously, like on Avalonia: the read is started when a target first asks whether the data contains the format
(drag enter or over), so its result is available when the data is dropped. **Done.**

### W13. Unhandled UI thread exceptions terminate the process

**Uno Platform:** the headless host swallows them. **WinUI:** the application terminates.
**Port:** the harness handles `Application.UnhandledException` and fails the running test. **Done.**

### W14. A code-only application resolves no XAML types

**WinUI:** an application not defined in XAML must resolve the XAML types itself (`IXamlMetadataProvider`), and its
`Resources` are not available in its constructor.
**Port:** the harness default application loads `XamlControlsResources` in `OnLaunched` and resolves the WinUI control
types; the test projects share a XAML application (`tests/WinUI/Shared/TestApp.xaml`) for which the XAML compiler
generates the type information of their test pages. **Done, to be verified.**

### W15. ReactiveUI and toolkit packages

| Uno Platform | WinUI |
|--------------|-------|
| `ReactiveUI.Uno.Reactive`: views and `RoutedViewHost` in `ReactiveUI.Uno.Reactive`, `WithUno(window)` | `ReactiveUI.WinUI.Reactive`: views and `RoutedViewHost` in `ReactiveUI.Reactive`, `WithWinUI()`. The sample uses its own `SampleRoutedViewHost` so its XAML does not depend on the namespace. |
| `Uno.CommunityToolkit.WinUI.UI.Controls.DataGrid`: resources registered through `GlobalStaticResources` | `CommunityToolkit.WinUI.UI.Controls.DataGrid`: the framework loads the toolkit resources |

### W16. The generator tests need the WinUI references

**Uno Platform:** the generator tests compile the generated code against the Uno.WinUI packages.
**Port:** the WinUI generator tests compile it against the Windows App SDK, Windows SDK and CsWinRT references of the
test project. **Done, to be verified.**

### W17. Default values of `RenderTransform` and `Transitions`

**Uno Platform:** `UIElement.RenderTransform` and `UIElement.Transitions` are `null` until set.
**WinUI:** `RenderTransform` returns an identity `MatrixTransform` and `Transitions` an empty collection when not set.
**Port:** the behaviors already replace any transform that is not a `TranslateTransform`; the tests check that no local
value is set (`ReadLocalValue`) or accept an empty collection on WinUI. **Done.**

### W18. Storyboards complete on a frame

**Uno Platform (headless):** running the queued work completes a zero duration storyboard.
**WinUI:** storyboards advance on the frames of the UI thread.
**Port:** `WinUITestSession.RenderFrame()` runs the queued work and waits for a rendered frame; the `RunJobs` of the
shared tests uses it on WinUI. **Done, to be verified.**

### W19. `Background` is not a `FrameworkElement` property

**Uno Platform:** `FrameworkElement` declares `Background` (`Control.BackgroundProperty` through the port alias).
**WinUI:** `Background` is declared by `Control`, `Panel`, `Border` and a few others.
**Port:** the sample uses `Microsoft.UI.Xaml.Controls.Control.BackgroundProperty` on WinUI. **Done.**

### W20. Dependency property types

**Uno Platform:** a dependency property can be registered with any type, and XAML can set a property of any type.
**WinUI:** native WinUI resolves the type a dependency property is registered with through the XAML type information
of the application, unless it is a type WinUI knows itself.

* The type information describes a system type that is not a WinUI class (`DependencyProperty`, `RoutedEvent`,
  `Window`, `IValueConverter`, `System.Type`, ...) without a base type: when WinUI asks for it, the application
  terminates (stowed exception `0xc000027b`, `NotImplementedException` in `XamlSystemBaseType.BaseType`). Found with
  the Behaviors sample.
* The type information only knows the types the XAML of the application uses. Any other type of a library or of the
  application is opaque: a dependency object stored in a property registered with it does not join the tree of its
  owner, so its bindings get no data context and resolve no element names. Behaviors added from code had bindings
  without a data context, as had the actions of a trigger and the cases of a switch.

**Port:** on native WinUI `Xaml.PropertyGenerator` registers the properties as `object`, except for the simple value
types, `string`, the WinUI structures and the WinUI dependency object classes (the CLR property keeps its type, so
XAML still converts strings). `Interaction.Behaviors` is registered as a `DependencyObjectCollection`. Uno Platform
registrations are unchanged. **Done.**

**Open:** a CLR property typed `DependencyProperty` or `RoutedEvent` cannot be set from a XAML attribute on native
WinUI (`ChangeAvaloniaPropertyAction.TargetProperty`, `BindingBehavior.TargetProperty`, `Condition.Property`,
`PropertyValidationBehavior.Property`, `RoutedEventTriggerBehavior.RoutedEvent`): loading the XAML fails with
"Cannot deserialize XBF metadata property list as 'TargetProperty' was not found", because WinUI cannot describe the
property type. Uno Platform converts the `Type.Property` text at build time. The pages of the Behaviors sample that
set these properties in XAML do not load on WinUI yet; setting the property from code works.

### W21. `FrameworkElement.IsLoaded` after a `Loaded` handler is added

**Uno Platform:** `IsLoaded` tells whether the element is in a live tree.
**WinUI:** adding the first `Loaded` handler to an element that is already loaded makes `IsLoaded` return `false`,
and it stays `false`. Attaching behaviors to a loaded element adds such a handler, so behaviors added to a loaded
element did not catch up with the lifecycle (`OnAttachedToVisualTree`, `OnLoaded`), and `ViewportBehavior` and the
event triggers considered the element unloaded.
**Port:** `LoadedState` (Interactivity compat) reads `IsLoaded` before the handlers are added and then tracks the state
with the `Loaded` and `Unloaded` events; the behaviors use it instead of `IsLoaded`. The WinUI test session does the
same when it shows an element. **Done.**

### W22. Element tree, events and input timing

| Uno Platform | WinUI | Port |
|--------------|-------|------|
| `Unloaded` is raised while the element is removed | `Unloaded` is raised later, on the UI thread (`IsLoaded` is `false` at once) | The detach lifecycle of the behaviors arrives with the event. The shared tests run the queued work after removing an element. **Done.** |
| `FrameworkElement.Parent` is set as soon as the element is added to a parent | `Parent` (and `VisualTreeHelper.GetParent`) is `null` until the tree is live | `RemoveElementAction` (and the other actions that use `Parent`) work on elements of a live tree. The tests show the elements. **Done.** |
| The container of a directly added item has the item as its data context | The container has no data context | `RemoveItemInListBoxAction` uses `ItemFromContainer`. **Done.** |
| `TransitionsChangedTrigger` runs its actions for the current collection when it is attached | The same, but an exception thrown by an action in a dispatcher callback terminates the process, and the `x:Bind` values of a view that is not shown (the content of a tab that is not selected) are never set | The current collection is not reported for an element that is not loaded (both WinUI platforms); changes are always reported. **Done.** |
| The default automation name is `null` | The default is an empty string | Test expectation. **Done.** |
| `ElementName` bindings are resolved when `Loaded` is raised | They are resolved after the `Loaded` event is dispatched | The actions of a `Loaded` (or default) event trigger run on the dispatcher, after the event, so that their `ElementName` bindings have a value. **Done.** |
| A popup, tool tip or flyout opens and closes with the queued work | It opens and closes over several frames, and a flyout cannot be shown again before it is closed | The shared tests wait for the `Opened`/`Closed` events. **Done.** |
| `DoubleTapped` is raised when the pointer is released | It is raised on the second press, and the focus moves to the root when the pointer is then released over content that cannot be focused | `InlineEditBehavior` focuses its editor once the pointer is released. **Done.** |
| The focus stays on the pressed element | A pointer press can move the focus to the root of the window | `ContextDragBehavior` handles the Escape key that cancels a pending drag at the root of the window. **Done.** |
| A text only container is hit on its whole area | Only the text is hit (no background) | Test page layout. **Done.** |
| An element can be added to a second parent | It throws | The tests remove the element first. **Done.** |
| Focus navigation follows the tab indexes inside a panel with local tab navigation | `FocusManager.FindNextElement` leaves the panel | One shared test of `FocusNextElementAction` is skipped. **Open (platform).** |

### W23. Bindings

| Uno Platform | WinUI | Port |
|--------------|-------|------|
| A binding observes the dependency properties of any `DependencyObject` source | The dependency properties of a source type that is not in the XAML type information of the application (a type not used in XAML) are read once and not observed | Sources of such bindings implement `INotifyPropertyChanged`, or the binding is an `x:Bind`. Two shared tests use a notifying source or do not test the update. **Documented.** |
| The change of a property is raised when the new value can be read | `FrameworkElement.Transitions` raises its change before `GetValue` returns the new collection | `TransitionOperations.Observe` reports such a change once it is applied (on the dispatcher). **Done.** |
| The properties of a dependency object can be read on any thread | They can only be read on the UI thread | The generated change property actions with `UseDispatcher` resolve their target on the UI thread. **Done.** |
| A local value set while a temporary (animation) value is effective is kept below it ([W4](#w4-no-value-precedences)) | No precedences | `AnimationValueLayer` observes the property: a different local value becomes the value to restore, and the temporary value stays effective. **Done.** |

### W24. Event arguments are not `System.EventArgs`

**Uno Platform:** `RoutedEventArgs` derives from `System.EventArgs`, and events without data pass `EventArgs.Empty`.
**WinUI:** event arguments are WinRT objects: `RoutedEventArgs` does not derive from `System.EventArgs`, and the
argument of events such as `Flyout.Opened` is a plain object.
**Port:** no change in the libraries. A method called by `CallMethodAction` with the `(object sender, EventArgs e)`
signature is not found on WinUI: declare the second parameter as the WinUI argument type or as `object`.
**Documented.**

### W25. A `Canvas` moves its children without a layout pass

**Uno Platform:** changing `Canvas.Left`/`Canvas.Top` raises `LayoutUpdated`.
**WinUI:** the child is moved and no `LayoutUpdated` event is raised.
**Port:** `FluidMoveBehavior` does not animate children of a `Canvas` moved with `Canvas.Left`/`Canvas.Top` on WinUI
(panels that lay out their children are animated). **Open (limitation).**

### W26. Packaging and trimming

The WinUI libraries are marked trimmable and AOT compatible like the other ports. CsWinRT generates code for the types
that implement WinRT interfaces: those types are `partial` (on all platforms) and the WinUI projects allow unsafe code.
The test projects and samples are unpackaged applications with a self-contained Windows App SDK for the architecture of
the machine (`win-x64` or `win-arm64`).

## Building and testing

```bash
dotnet build WinUIBehaviors.slnx -c Release
```

```bash
powershell -File build/WinUIPort/run-tests.ps1
```

The test runner runs the test projects one at a time on the interactive desktop (do not use the mouse or keyboard
while it runs), stops a test project that does not finish within `-TimeoutMinutes` (15 by default) and writes the
output of each project to `artifacts/winui-tests`.
