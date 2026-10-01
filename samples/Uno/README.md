# Uno Platform samples

Every Avalonia sample application has a Uno Platform twin with the same name and structure under `samples/Uno`:

| Avalonia | Uno Platform |
|----------|--------------|
| `samples/BehaviorsTestApplication` | `samples/Uno/BehaviorsTestApplication` |
| `samples/AnimationsTestApplication` | `samples/Uno/AnimationsTestApplication` |
| `samples/SourceGeneratorSample` | `samples/Uno/SourceGeneratorSample` |

## Status

| Avalonia sample | Views | Uno Platform twin |
|-----------------|------:|-------------------|
| `BehaviorsTestApplication` | 231 pages (234 tabs) + shell, ReactiveUI, dialog and control views | every view has a twin; 214 pages are real ports, 17 show `NotAvailableOnUnoView` |
| `AnimationsTestApplication` | 16 pages + shell | every page is a real port |
| `SourceGeneratorSample` | 11 pages + shell | every page is a real port (generated behaviors in `SourceGeneratorSample.Core`) |

`BehaviorsTestApplication` pages whose feature has no WinUI counterpart (see the status table of
[PORTING.md](../../src/Uno/PORTING.md)) show the reason instead of the sample:

| Page | Reason |
|------|--------|
| Add/RemoveClassAction, ToggleClassAction | Avalonia style classes and selectors |
| Cursor Behavior, Drawn Cursor | WinUI exposes the element cursor only through the protected `ProtectedCursor` |
| Gesture Advanced (pinch), Pull, Scroll and TouchPad gesture triggers | Avalonia gesture recognizers without a WinUI routed event |
| Notifications, ShowNotificationAction | Avalonia notification managers |
| NumericUpDownValidationBehavior | WinUI has no `NumericUpDown` |
| RenderTargetBitmap | immediate mode drawing into an Avalonia `RenderTargetBitmap` |
| ResourcesChangedBehavior, ResourcesChangedTrigger | WinUI raises no resources changed notification |
| Screens | Avalonia Screens API (`DisplayArea` is not implemented by Uno Platform) |
| VisualDebugBehavior | Avalonia adorner layer |
| WindowDragMoveBehavior | `Window.BeginMoveDrag` |

The "Managed (Preview)" tab of the DragAndDrop page is not available either (managed drag and drop, see PORTING.md).
Pages that are real ports adapt Avalonia-only details (controls, bindings, layout) with comments in their `.xaml`.

## How sharing works

A Uno sample is an `Uno.Sdk` application (`net10.0-desktop`, Skia desktop head) that shares the C# sources of its
Avalonia twin, like `src/Uno` shares the library sources (see [src/Uno/PORTING.md](../../src/Uno/PORTING.md)). It
sets `<UnoSharedSampleProject>` to the folder name of the Avalonia sample, imports its `SharedSources.props`
(`UnoSharedSampleExcludes`: files that are not compiled for Uno, with the reasons) and
[`UnoSharedSample.targets`](UnoSharedSample.targets), which compiles the Avalonia sample's C# sources (view models,
models, behaviors, converters, code-behind) with the `UNO` symbol and the port aliases of `src/Uno`
(`Control` = `FrameworkElement`, `Point`, `Key`, `DragDropEffects`, …). `App` and `Program` are Uno-only (the
composition root and the desktop head).

**The code-behind of a view (`X.axaml.cs`) is compiled only when its WinUI twin (`X.xaml`, same relative path next to
the Uno project) exists.** Views that are not ported yet are therefore excluded automatically; porting a view never
edits a shared list. Use `SharedSources.props` only for code-behind that cannot be shared although the view has a twin.

Files that only exist on Uno live next to the Uno project at the path of their Avalonia counterpart (for example
`Behaviors/DropHandlerCompat.cs`, `Views/ReactiveUI/ReactiveViewBases.cs`, `Controls/PendingSampleView.xaml`).

## BehaviorsTestApplication

* `App.xaml` merges `SideBar.xaml`, `Styles/DraggableCustomStyles.xaml` and `Controls/CustomControl.xaml` and defines
  the sample brushes with the keys of the Avalonia `App.axaml` (`BlackBrush`, `WhiteBrush`, `GrayBrush`, `RedBrush`,
  `GreenBrush`, `BlueBrush`, `YellowBrush`, `PinkBrush`; Avalonia `{DynamicResource PinkBrush}` →
  `{StaticResource PinkBrush}`). Buttons stretch like in the Avalonia sample.
* `App.xaml.cs` is the composition root: the WinUI `Window`, `RxAppBuilder…WithUno(window)` and
  `MainView { DataContext = new MainWindowViewModel() }` (a WinUI window has no data context; the `MainWindow`
  behaviors run on `MainView`).
* `Views/MainView.xaml` is **generated** by [`build/UnoPort/generate_sample_mainview.py`](../../build/UnoPort/generate_sample_mainview.py)
  from the Avalonia `MainView.axaml`: the same 234 tabs with the same headers in the same order, in a `TabView`
  (`SingleSelectionTabControl` derives from `TabView` on Uno) re-templated as a vertical sidebar (`SideBar.xaml`:
  `SidebarTabViewStyle`, `SidebarTabViewItemStyle`), filtered and sorted by `SelectingItemsControlSearchBehavior`. A tab
  hosts `<pages:XView />` when `XView.xaml` exists anywhere under `Views/`, otherwise a `PendingSampleView`. Never edit
  it by hand:

  ```bash
  python3 build/UnoPort/generate_sample_mainview.py          # regenerate after adding a page twin
  python3 build/UnoPort/generate_sample_mainview.py --check  # fails when MainView.xaml is out of date
  ```

## Porting a page

Every Avalonia view (`.axaml`) gets a WinUI twin (`.xaml`) at the same relative path next to the Uno project; both
share the code-behind (`.axaml.cs`). `Views/Pages/CallMethodActionView`, `ChangePropertyActionView` and
`InvokeCommandActionView` are complete examples.

1. Run `python3 build/UnoPort/uno_share.py samples/BehaviorsTestApplication/Views/Pages/XView.axaml.cs` (plus any other
   C# file the page needs). It puts `using Avalonia.*` directives into `#if UNO … #else … #endif` blocks (namespaces stay
   `BehaviorsTestApplication.*`), casts hand-written property getters and wraps the Avalonia
   `private void InitializeComponent() { AvaloniaXamlLoader.Load(this); }` in `#if !UNO` (the Uno XAML generator
   provides `InitializeComponent` from the twin). Do not run it on whole folders: it would touch pages owned by others.
2. For compiled bindings add a typed view model accessor to the code-behind (the only Uno addition a view needs):

   ```csharp
   #if UNO
       /// <summary>
       /// Gets the view model for the compiled bindings (x:Bind) of the Uno Platform view.
       /// </summary>
       public MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;
   #endif
   ```

   (`using BehaviorsTestApplication.ViewModels;` goes into the `#if UNO` using block). Use the `x:DataType` of the
   Avalonia view; x:Bind is evaluated when the view loads, after the data context is inherited or set in the
   constructor. Views keep no logic beyond `InitializeComponent()` (MVVM, see AGENTS.md); anything else Avalonia-only in
   the code-behind goes into `#if !UNO` with a comment (or a `#if UNO` counterpart).
3. Write `samples/Uno/BehaviorsTestApplication/Views/Pages/XView.xaml` with the same structure, element names, texts and
   behaviors:
   * `using:` XML namespaces: `i` = `Xaml.Interactivity`, `ic` = `Xaml.Interactions.Core`,
     `icustom` = `Xaml.Interactions.Custom`, `ie` = `Xaml.Interactions.Events`, `idd` = `Xaml.Interactions.DragAndDrop`,
     `idrag` = `Xaml.Interactions.Draggable`, `ir` = `Xaml.Interactions.Responsive`, `irx` = `Xaml.Interactions.ReactiveUI`,
     `controls` = `BehaviorsTestApplication.Controls`, `behaviors` = `BehaviorsTestApplication.Behaviors`, … (Avalonia's
     default namespace maps all of them),
   * compiled bindings: `{x:Bind ViewModel.Count, Mode=OneWay}` (x:Bind defaults to OneTime; WinUI has no default
     TwoWay modes, write `Mode=TwoWay` where Avalonia relies on one), `{x:Bind ElementName}` instead of
     `SourceObject="ElementName"`/`TargetObject="ElementName"` (`[ResolveByName]`), `#name`, `$parent[…]`;
     `{Binding … RelativeSource={RelativeSource TemplatedParent}}` only inside control templates,
   * no `StringFormat`: call a static formatter (`{x:Bind converters:SampleFormat.Value(Observer.Value), Mode=OneWay}`,
     `Converters/SampleFormat.cs`); no `x:Static`: `{x:Bind ns:Type.Member}`; no `x:TypeArguments`: close generic
     behaviors in `Behaviors/ClosedGenericBehaviors.cs` (`Int32ObservableTriggerBehavior`, `NavigateToDetailPageAction`,
     …); `ReactiveUserControl<T>` views derive from a closed base (`Views/ReactiveUI/ReactiveViewBases.cs`),
   * WinUI controls and properties: `Grid` row/column definitions as elements, `ListView` for `ListBox` (the shared drop
     handlers alias `ListBox` to `ListView`), `TabView`/`TabViewItem` for `TabControl`/`TabItem`, `AutoSuggestBox`,
     `ContentDialog` (`Dialogs/SimpleDialog`), `Visibility` for `IsVisible`, event names `Loaded`/`Unloaded` for
     `AttachedToVisualTree`/`DetachedFromVisualTree`, visual states instead of style classes, keyed styles instead of
     selectors (`CustomTabViewStyle`/`CustomTabViewItemStyle` for the `custom` class of `DraggableCustomStyles`); there
     is no `WrapPanel`/`DockPanel`; keep the Avalonia look where practical,
   * images: `ms-appx:///Assets/<file>` (the Avalonia `Assets` are linked into the Uno project).
4. A page whose behavior has no WinUI counterpart (see the status table of PORTING.md) still gets a twin: its content
   is a `controls:NotAvailableOnUnoView` with the same `Title` and a `Reason`, and its code-behind stays shared (wrap
   Avalonia-only code in `#if !UNO`). Example: `Views/ReactiveUI/InteractionTriggerBehaviorView.xaml`.
5. Run `python3 build/UnoPort/generate_sample_mainview.py`, build both samples
   (`dotnet build UnoBehaviors.slnx -c Release`, `dotnet build AvaloniaBehaviors.slnx -c Release`) and run the Uno
   sample (`dotnet run --project samples/Uno/BehaviorsTestApplication -c Release`); every page is instantiated at
   startup, so an exception in a page constructor stops the app. Check that the page renders and works.

### Known limitations

* The samples use the System.Reactive flavor of ReactiveUI 25.1 (`ReactiveUI.Avalonia.Reactive`, `ReactiveUI.Uno.Reactive`,
  types in `ReactiveUI.Reactive` and `ReactiveUI.Binding.Reactive`), like the ReactiveUI behaviors. Generic ReactiveUI
  behaviors are closed in `Behaviors/ClosedGenericBehaviors.cs` (`UnitInteractionTriggerBehavior`).
* The Uno sample references `System.Reactive` explicitly (ReactiveUI 25 no longer depends on it).

## Source generated behaviors in XAML

The Uno XAML generator is a Roslyn source generator, and source generators do not see each other's output: a WinUI
view cannot use actions and triggers generated by `Xaml.Behaviors.SourceGenerators` in its own assembly
(`UXAML0001`, the type could not be found). The Uno twin of `samples/SourceGeneratorSample` therefore compiles the
shared view models, models and converters, with the generator in its WinUI mode, in a library
(`samples/Uno/SourceGeneratorSample.Core`, which excludes `Views/**`), and the application
(`samples/Uno/SourceGeneratorSample`, which excludes `ViewModels/**`, `Models/**` and `Converters/**`) compiles the
views and references it. The assembly attributes of the generated framework actions and triggers name the WinUI types
under `#if UNO` (for example `Button.Click` is declared on `ButtonBase`, so the trigger is
`Microsoft.UI.Xaml.Controls.Primitives.ButtonBaseClickTrigger`).
