# WinUI tests

The WinUI test projects run the test suites of the Uno Platform port (`tests/Uno`), which themselves share the Avalonia
tests (see [tests/Uno/README.md](../Uno/README.md)), on native WinUI 3. Every Uno test project has a WinUI twin with
the same folder name; the twin has no test sources of its own except for the tests of the WinUI test harness.

| Uno Platform | WinUI |
|--------------|-------|
| `tests/Uno/Xaml.Behaviors.<Name>/` | `tests/WinUI/Xaml.Behaviors.<Name>/` (xUnit v3 exe, `Xaml.Behaviors.WinUI.<Name>` assembly) |
| `[UnoHeadlessFact]`, `[UnoHeadlessTheory]` | `[WinUIFact]`, `[WinUITheory]` (global aliases) |
| `UnoHeadlessSession`, `.Keyboard`, `.Mouse` | `WinUITestSession`, `WinUITestKeyboard`, `WinUITestMouse` (global aliases) |
| XAML pages compiled by the Uno XAML generator | the same `.xaml` files compiled by the WinUI XAML compiler |

## Running

The tests need Windows 10 1809 or later and an **interactive desktop**: WinUI has no headless mode, so the session shows
a real (topmost) window, and keyboard and mouse input is injected through the operating system. Do not use the mouse or
keyboard while the tests run, and do not run two test projects at the same time.

```powershell
dotnet build WinUIBehaviors.slnx -c Release
build\WinUIPort\run-tests.ps1                      # all projects, results in artifacts\winui-tests
build\WinUIPort\run-tests.ps1 -Filter Draggable    # one project
```

A test project is an unpackaged, self-contained Windows App SDK executable (no runtime to install); it can also be
started directly, with the xUnit v3 command line (`-method`, `-class`, ...).

## How sharing works

A WinUI test project sets `<WinUIUnoTestProject>` to the folder name of its Uno twin and imports
[`WinUISharedTests.targets`](WinUISharedTests.targets), which

* compiles the sources and the XAML pages of the Uno test project as links,
* imports the `SharedTests.props` of the Avalonia test project and `UnoSharedTests.targets`, so the Avalonia tests are
  shared exactly like in the Uno twin,
* maps the Uno headless types to the WinUI test harness with global aliases,
* uses the application of [`Shared`](Shared) (`TestApp.xaml`: the XAML type information of the test assembly and the
  WinUI control styles) and tracks the test window (`WindowTracker`).

Use `#if WINUI` in a test for a native WinUI difference, and record it in
[winui-differences.md](../../docfx/articles/winui/winui-differences.md). A test that cannot run on native WinUI is
skipped with `Assert.Skip` and a reason, not removed.

## Differences that affect tests

* Input is real: a test fails when another window covers the test window (the failure message names it).
* Animations and transitions run in the compositor: a test cannot step them, only wait for their end.
* An unhandled exception on the UI thread is reported as a failure of the running test.
* A drag and drop operation runs a modal loop: the mouse is driven from another thread (`MoveToAsync`, ...).
