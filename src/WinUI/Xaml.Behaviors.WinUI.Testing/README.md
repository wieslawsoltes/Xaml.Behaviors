# Xaml.Behaviors.WinUI.Testing

Test harness for WinUI 3 (Windows App SDK) applications, used by the WinUI port of XAML Behaviors.

- `WinUITestSession` runs a WinUI application and a test window on a dedicated UI thread for the lifetime of the
  test process. `Show`/`ShowAsync` show an element as the window content, `RunJobs` runs the queued UI work (the
  pending messages of the UI thread) and the layout, `RunAsync` runs code on the UI thread.
- `Keyboard` and `Mouse` inject real input (`SendInput`) into the test window, which is brought to the foreground
  first: the tests need an interactive desktop, and the user must not use the mouse or keyboard while they run.
- `Xaml.Behaviors.WinUI.Testing.XUnit` adds `[WinUIFact]` and `[WinUITheory]` (xUnit v3), which run tests on the UI
  thread.

The API matches the Uno Platform headless harness (`Xaml.Behaviors.Uno.Headless`), so tests can be shared by both
ports.
