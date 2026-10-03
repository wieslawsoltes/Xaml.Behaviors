# WinUI samples

The WinUI samples are the Uno Platform samples (see [samples/Uno/README.md](../Uno/README.md)) built for native
WinUI 3: every project links the sources and the XAML pages of its Uno twin and has no source of its own.

| Uno Platform | WinUI |
|--------------|-------|
| `samples/Uno/BehaviorsTestApplication` | `samples/WinUI/BehaviorsTestApplication` |
| `samples/Uno/AnimationsTestApplication` | `samples/WinUI/AnimationsTestApplication` |
| `samples/Uno/SourceGeneratorSample` (+ `.Core`) | `samples/WinUI/SourceGeneratorSample` (+ `.Core`) |

## Running

The samples build on Windows only (WinUI XAML compiler). They are unpackaged and self-contained:

```powershell
dotnet run --project samples\WinUI\BehaviorsTestApplication -c Release
```

## How sharing works

A sample project sets `<WinUIUnoSampleProject>` to the folder name of its Uno twin and imports
[`WinUISharedSample.targets`](WinUISharedSample.targets), which links the `.cs` files of the Uno sample (without its
`Platforms` folder), its `.xaml` files as pages, its `App.xaml` as the application definition and the sources shared
with the Avalonia sample.

The window size of a WinUI application is in physical pixels: the samples scale it with the display scale.

The WinUI XAML compiler is stricter than the Uno XAML generator. A page of the Uno samples must stay valid for both;
the rules found so far are listed in
[winui-differences.md](../../docfx/articles/winui/winui-differences.md) (attached property getters, `x:Bind` to
nullable values, panels of the WinUI library only, `x:Bind` for properties typed `DependencyProperty`, ...).
