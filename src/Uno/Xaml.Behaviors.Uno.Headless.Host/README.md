# Xaml.Behaviors.Uno.Headless.Host

A headless (offscreen) Skia host for [Uno Platform](https://platform.uno), repackaged from the Uno
Platform sources (`src/Uno.UI.Runtime.Skia.Headless`, Apache-2.0, see `NOTICE.md`) because the official
`Uno.WinUI.Runtime.Skia.Headless` package is not published on nuget.org yet.

Most consumers should reference `Xaml.Behaviors.Uno.Headless` (session/harness) or
`Xaml.Behaviors.Uno.Headless.XUnit` (`[UnoHeadlessFact]` / `[UnoHeadlessTheory]`) instead of using
this package directly.

```csharp
UnoPlatformHostBuilder.Create()
    .App(() => new App())
    .UseHeadless(headless => headless.WithSize(1024, 768).WithScale(1f))
    .Build()
    .Run(); // Blocks until the application exits: run it on a dedicated thread.
```

## Why the assembly name is fixed

The assembly is named `Uno.UI.Runtime.Skia.Headless` (not after the package id) because the Uno Skia
runtime assemblies (`Uno.UI`, `Uno`, `Uno.Foundation`, `Uno.UI.Dispatching`, `Uno.UI.Composition`)
grant `InternalsVisibleTo` to that name, and the host relies on those internals. Do not reference this
package together with the official `Uno.WinUI.Runtime.Skia.Headless` package: both produce the same assembly.

## Exact Uno version pin

Because it binds to Uno internals, the package depends on **exactly** the `Uno.WinUI` and
`Uno.WinUI.Runtime.Skia` versions it was compiled against (for example `[6.7.135]`). Any other Uno
version fails to restore; update this package together with Uno.

## What the package adds to a consuming project

`buildTransitive` sets `UnoRuntimeIdentifier=Skia` (so the Skia runtime assemblies are copied to the
output instead of the reference API ones) and marks executable projects (for example xUnit v3 test
projects) as Uno heads.
