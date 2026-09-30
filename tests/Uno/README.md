# Uno Platform tests

The Uno Platform port runs the Avalonia test suites on Uno Platform by sharing their sources, like `src/Uno` shares the
library sources (see [src/Uno/PORTING.md](../../src/Uno/PORTING.md)). Every Avalonia test project has a Uno twin with
the same name under `tests/Uno`; the twin also keeps its Uno-only tests.

| Avalonia | Uno Platform |
|----------|--------------|
| `tests/Xaml.Behaviors.<Name>/` | `tests/Uno/Xaml.Behaviors.<Name>/` (Uno.Sdk xUnit v3 exe, `Xaml.Behaviors.Uno.<Name>` assembly) |
| `[AvaloniaFact]`, `[AvaloniaTheory]` | `[UnoHeadlessFact]`, `[UnoHeadlessTheory]` (global aliases, the test runs on the Uno UI thread) |
| `.axaml` test page + `.axaml.cs` | `.xaml` page next to the Uno project (same relative path) + the shared `.axaml.cs` |

## Status

Every Avalonia test suite is shared with its Uno twin. Tests excluded on Uno test features that have no WinUI counterpart;
the reasons are next to each exclusion (`SharedTests.props` or `#if !UNO`).

| Avalonia test project | Tests | Run on Uno | Excluded on Uno |
|-----------------------|------:|-----------:|-----------------|
| `Xaml.Behaviors.Interactivity.UnitTests` | 130 | 121 | 9: action collection templates (element styles), TopLevel close, `AttachedToVisualTree`/`Initialized` events, action initialization state |
| `Xaml.Behaviors.Interactions.UnitTests` | 158 | 144 | 14: direct (field-backed) properties, managed drag and drop, runtime XAML loader, `WindowClosedEvent`, tunneling pointer route, `KeyboardNavigationMode.None`, handled click events, tool tip opening/closing events |
| `Xaml.Behaviors.Animations.UnitTests` | 43 | 43 | – |
| `Xaml.Behaviors.SourceGenerators.UnitTests` | 232 | 231 | 1: Avalonia value priorities (runtime); the generator scenarios run through the WinUI emitter |
| `Xaml.Behaviors.SourceGenerators.IntegrationTests` | 2 | – | NuGet package scenarios, skipped on Avalonia as well |

The Uno twins also keep their Uno-only tests. Two drag reorder tests that are skipped on the Avalonia headless platform
run on Uno Platform, where the headless session injects real pointer input.

## How sharing works

A Uno test project is an `Uno.Sdk` project (so its XAML pages are compiled by the Uno XAML generator) with
`OutputType=Exe` and xUnit v3. It sets `<UnoSharedTestProject>` to the folder name of the Avalonia test project,
imports its `SharedTests.props` (`UnoSharedTestExcludes`: files that are not compiled for Uno) and
[`UnoSharedTests.targets`](UnoSharedTests.targets), which

* compiles the Avalonia test sources with the `UNO` symbol (except `TestAppBuilder.cs`, `App.axaml.cs` and the
  excluded files),
* imports the port aliases of `src/Uno` (`Control` → `FrameworkElement`, `Point`, `Key`, …),
* compiles the Avalonia headless test compat layer [`Shared/TestCompat`](Shared/TestCompat) and adds the aliases
  `AvaloniaFact` → `UnoHeadlessFact`, `AvaloniaTheory` → `UnoHeadlessTheory` and `Window` → `HeadlessTestWindow`.

Projects that do not reference `Xaml.Behaviors.Uno.Interactivity` set
`<UnoSharedTestsUseInteractivity>false</UnoSharedTestsUseInteractivity>`.

## Test compat layer

| Avalonia headless API | Uno Platform |
|-----------------------|--------------|
| `new Window { Content = … }`, `Show()`, `Close()`, `Title` | `HeadlessTestWindow`: shown as the content of the session window (`UnoHeadlessSession.Show`) |
| `Dispatcher.UIThread.RunJobs()` | `UnoHeadlessSession.RunJobs()`: runs the queued UI work synchronously |
| `CaptureRenderedFrame()?.Save(…)` | runs the queued work and the layout; no image (returns `null`) |
| `KeyPressQwerty(PhysicalKey, RawInputModifiers)` | a key press **and release** on the focused element (WinUI buttons click on key up) |
| `MouseDown/MouseUp/MouseMove/MouseWheel(point, …)` | `UnoHeadlessSession.Mouse` (absolute positions relative to the element) |
| `FindControl<T>(name)` | `FrameworkElement.FindName` |
| `TranslatePoint(point, relativeTo)` | `TransformToVisual` (`CoreTestCompat`) |

Extend the compat layer when an Avalonia test API is used in several tests; use `#if UNO` in the test otherwise.
Keyboard and mouse modifiers of mouse events are not simulated. Like a new Avalonia window, `HeadlessTestWindow.Show`
starts a separate input sequence (clicks of a previous test do not continue into a double tap).

## Source generator tests

`Xaml.Behaviors.SourceGenerators.UnitTests` shares both kinds of tests of the Avalonia project:

* runtime tests (`[AvaloniaFact]`) run the WinUI code the generator emits for the Uno project itself (the generator is
  an analyzer of the project) on the headless session,
* generator tests (`[Fact]`) run the same Avalonia-targeted scenarios through the WinUI emitter: under `UNO`,
  `GeneratorTestHelper.RunGenerator` compiles them against the Uno.WinUI reference assemblies and
  `Xaml.Behaviors.Uno.Interactivity` after translating the few Avalonia APIs they use
  ([`WinUISourceTranslator`](Xaml.Behaviors.SourceGenerators.UnitTests/WinUISourceTranslator.cs)). Expectations on
  the generated code that differ on WinUI (dependency property registrations, `PostOnUIThread`, `global::` base types,
  WinUI diagnostics such as XBG036) are asserted in `#if UNO` blocks.

The Uno-only generator tests live in its `WinUI` folder.

## Porting a test file

1. Run `python3 build/UnoPort/uno_share.py <files>` on the Avalonia test files (namespaces `Avalonia.Xaml.X` →
   `Xaml.X`, `using Avalonia.*` → `#if UNO` blocks). The Avalonia build must stay unchanged: everything Uno-specific
   lives in `#if UNO` blocks.
2. Remove the files from the "not ported yet" block of `SharedTests.props`.
3. For every `.axaml` page, add a `.xaml` page at the same relative path next to the Uno project:
   * root element `<compat:HeadlessTestWindow xmlns:compat="using:Xaml.Behaviors.Uno.TestCompat" …>` (or the page's
     root type) with `x:Class` in the `Xaml.*` namespace,
   * `x:Name="…" x:FieldModifier="internal"` for every element the tests access (Avalonia's generated fields are
     internal, Uno's are private by default),
   * WinUI XAML: `using:` namespaces (`i:` = `Xaml.Interactivity`, `ic:` = `Xaml.Interactions.Core`, …),
     `{x:Bind}` or `{Binding ElementName=…}` instead of `$parent[…]` and `#name` bindings, WinUI controls and
     properties. Keep the element names, structure and behavior configuration of the Avalonia page.
4. Build and run the Uno project; adapt differences with `#if UNO` (keep the assertion of the Avalonia test whenever
   the behavior is the same).
5. Tests of features that have no WinUI counterpart (see the PORTING.md status table) stay excluded: keep the file in
   `SharedTests.props` with the reason, or wrap single tests in `#if !UNO` with a comment.
6. Verify both sides: the Avalonia test project and the Uno twin pass.
