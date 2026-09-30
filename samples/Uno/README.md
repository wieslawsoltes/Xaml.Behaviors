# Uno Platform samples

Every Avalonia sample application has a Uno Platform twin with the same name and structure under `samples/Uno`:

| Avalonia | Uno Platform |
|----------|--------------|
| `samples/BehaviorsTestApplication` | `samples/Uno/BehaviorsTestApplication` |
| `samples/AnimationsTestApplication` | `samples/Uno/AnimationsTestApplication` |
| `samples/SourceGeneratorSample` | `samples/Uno/SourceGeneratorSample` |

## How sharing works

A Uno sample is an `Uno.Sdk` application (`net10.0-desktop`, Skia desktop head) that shares the C# sources of its
Avalonia twin, like `src/Uno` shares the library sources (see [src/Uno/PORTING.md](../../src/Uno/PORTING.md)). It
sets `<UnoSharedSampleProject>` to the folder name of the Avalonia sample, imports its `SharedSources.props`
(`UnoSharedSampleExcludes`: files that are not compiled for Uno) and [`UnoSharedSample.targets`](UnoSharedSample.targets),
which compiles the Avalonia sample's C# sources (view models, models, behaviors, converters, code-behind) with the `UNO`
symbol and the port aliases of `src/Uno`. `App` and `Program` are Uno-only (the composition root and the desktop head).

## Porting a view

Every Avalonia view (`.axaml`) gets a WinUI twin (`.xaml`) at the same relative path next to the Uno project; both
share the code-behind (`.axaml.cs`):

1. Run `python3 build/UnoPort/uno_share.py <code-behind and C# files>` (namespaces stay `BehaviorsTestApplication.*`;
   `using Avalonia.*` directives go into `#if UNO` blocks). In the code-behind, wrap the Avalonia
   `InitializeComponent()`/`AvaloniaXamlLoader.Load(this)` in `#if !UNO` (the Uno XAML generator provides
   `InitializeComponent`). Views keep no logic beyond `InitializeComponent()` (MVVM, see AGENTS.md).
2. Write the `.xaml` twin with the same structure, element names, texts and behaviors:
   * `using:` XML namespaces: `i` = `Xaml.Interactivity`, `ic` = `Xaml.Interactions.Core`,
     `icustom` = `Xaml.Interactions.Custom`, `ie` = `Xaml.Interactions.Events`, `idd` = `Xaml.Interactions.DragAndDrop`,
     `idrag` = `Xaml.Interactions.Draggable`, `ir` = `Xaml.Interactions.Responsive`, … (Avalonia's default
     namespace maps all of them),
   * compiled bindings (`{x:Bind}` against a typed `ViewModel` property of the view, or `{Binding}` with
     `x:DataType` where x:Bind cannot be used); `ElementName` instead of `#name`/`$parent[…]`,
   * WinUI controls and properties (`ListView`/`ItemsRepeater`, `TabView`, `AutoSuggestBox`, `ContentDialog`,
     `Visibility`, `ThemeResource`, visual states instead of style classes, …); keep the Avalonia look where
     practical,
   * resources: Avalonia `DynamicResource` brushes of the sample map to `ThemeResource`/`StaticResource` brushes
     defined in the Uno `App.xaml` with the same keys.
3. Remove the code-behind from the excludes in `SharedSources.props`.
4. A view whose behavior has no WinUI counterpart (see the status table of PORTING.md) still gets a twin: it shows the
   same title and a short explanation of why the sample is not available on Uno Platform (the
   `NotAvailableOnUnoView` control of the sample).
5. Build and run the app; check the view renders and works.
