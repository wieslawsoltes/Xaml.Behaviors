# WinUI 3 (Windows App SDK) port

The WinUI libraries are built from the sources of the Uno Platform port (`src/Uno`), which is itself built from the
Avalonia sources (`src/<Project>`): a WinUI project contains almost no code of its own.

| Avalonia | Uno Platform | WinUI |
|----------|--------------|-------|
| `src/Xaml.Behaviors.<Name>/` | `src/Uno/Xaml.Behaviors.<Name>/` | `src/WinUI/Xaml.Behaviors.<Name>/` |
| `Xaml.Behaviors.<Name>` | `Xaml.Behaviors.Uno.<Name>` | `Xaml.Behaviors.WinUI.<Name>` (assembly and package) |
| `AvaloniaBehaviors.slnx` | `UnoBehaviors.slnx` | `WinUIBehaviors.slnx` |

See [PORTING.md](../Uno/PORTING.md) for the rules of the Uno Platform port; they apply to WinUI as well. The
differences between the two WinUI platforms, and the status of the port, are tracked in
[winui-differences.md](../../docfx/articles/winui/winui-differences.md). **Update that document with every difference
found.**

## How source linking works

A WinUI project sets `<WinUIUnoProject>` to the folder name of its Uno Platform twin.
[`Directory.Build.targets`](Directory.Build.targets) then

* imports the `SharedSources.props` of the Uno project (the Avalonia sources that are not compiled for the WinUI API),
* compiles the shared Avalonia sources (`src/<Project>/**/*.cs`) and the Uno sources (`src/Uno/<Project>/**/*.cs`) as
  links,
* generates the `InternalsVisibleTo` attributes of the Uno project for the WinUI assembly names,
* adds `Microsoft.WindowsAppSDK`, the property generator, trimming, signing and Source Link.

[`Directory.Build.props`](Directory.Build.props) defines the symbols `UNO` (the WinUI API surface, shared with the Uno
Platform port) and `WINUI` (native WinUI only), and imports the port aliases of `src/Uno`.

Use `#if WINUI` in a shared or Uno source for a small difference. Put a type that only exists on native WinUI (for
example [`WindowTracker`](Xaml.Behaviors.Interactivity/WindowTracker.cs)) next to the WinUI project file, and exclude
the Uno source it replaces in the project file.

## Building

```bash
dotnet build WinUIBehaviors.slnx -c Release
```

The libraries compile on every operating system (`EnableWindowsTargeting`), which is enough to check a change on macOS
or Linux:

```bash
dotnet build src/WinUI/Xaml.Behaviors/Xaml.Behaviors.csproj -c Release
```

Projects with XAML pages (the test projects and the samples) need the WinUI XAML compiler and only build on Windows.

## Test harness

`Xaml.Behaviors.WinUI.Testing` (+ `.XUnit`) is the WinUI counterpart of `Xaml.Behaviors.Uno.Headless`: it starts a
WinUI application with one window on a dedicated UI thread and runs `[WinUIFact]`/`[WinUITheory]` tests on that thread.
There is no headless WinUI: the window is real, and keyboard and mouse input is injected through the operating system
(`SendInput`). See [tests/WinUI/README.md](../../tests/WinUI/README.md).
