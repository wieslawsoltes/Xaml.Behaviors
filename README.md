# XAML Behaviors


[![CI](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/build.yml)
[![Release](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/release.yml/badge.svg?branch=master)](https://github.com/wieslawsoltes/Xaml.Behaviors/actions/workflows/release.yml)

[![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Avalonia.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia)
[![NuGet](https://img.shields.io/nuget/dt/Xaml.Behaviors.Interactivity.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity)

**Xaml Behaviors** is a port of [Windows UWP](https://github.com/Microsoft/XamlBehaviors) version of XAML Behaviors for [Avalonia](https://github.com/AvaloniaUI/Avalonia) XAML.

XAML Behaviors for Avalonia is an easy-to-use means of adding common and reusable interactivity to your [Avalonia](https://github.com/AvaloniaUI/Avalonia) applications with minimal code. Avalonia port is available only for managed applications. Use of XAML Behaviors is governed by the MIT License.

In addition to the classic reflection-based behaviors from WPF/UWP, this port adds **source generators** that create strongly typed actions and triggers. They remove runtime reflection, improve AOT/linker friendliness, and surface diagnostics at compile time.

See the [AOT-friendly behaviors docs](docfx/articles/source-generators/index.md) for examples and guidance.

## Building XAML Behaviors Avalonia

First, clone the repository or download the latest zip.
```
git clone https://github.com/wieslawsoltes/Xaml.Behaviors.git
```

### Build on Windows using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=windows).

Open up a command-prompt and execute the commands:
```
.\build.ps1
```

### Build on Linux using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=linux).

Open up a terminal prompt and execute the commands:
```
./build.sh
```

### Build on OSX using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=macos).

Open up a terminal prompt and execute the commands:
```
./build.sh
```

## NuGet

Avalonia XamlBehaviors is delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/Xaml.Behaviors/) and install the package like this:

`Install-Package Xaml.Behaviors`

or by using nightly build feed:
* Add `https://www.myget.org/F/xamlbehaviors-nightly/api/v2` to your package sources
* Alternative nightly build feed `https://pkgs.dev.azure.com/wieslawsoltes/GitHub/_packaging/Nightly/nuget/v3/index.json`
* Update your package using `XamlBehaviors` feed

and install the package like this:

`Install-Package Xaml.Behaviors -Pre`

### Package Sources

* https://api.nuget.org/v3/index.json

### Available Packages

| Package | NuGet | Description |
|---------|-------|-------------|
| [Xaml.Behaviors](https://www.nuget.org/packages/Xaml.Behaviors) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.svg)](https://www.nuget.org/packages/Xaml.Behaviors) | Complete library of behaviors, actions and triggers for Avalonia applications. |
| [Xaml.Behaviors.Avalonia](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Avalonia.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia) | Meta package that bundles all Avalonia XAML Behaviors for easy installation. |
| [Xaml.Behaviors.Interactivity](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactivity.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity) | Foundation library providing base classes for actions, triggers and behaviors. |
| [Xaml.Behaviors.Interactions](https://www.nuget.org/packages/Xaml.Behaviors.Interactions) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions) | Core collection of common triggers and actions for Avalonia. |
| [Xaml.Behaviors.Interactions.Custom](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Custom) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Custom.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Custom) | Custom behaviors and actions for common Avalonia controls. |
| [Xaml.Behaviors.Interactions.DragAndDrop](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.DragAndDrop.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop) | Behaviors that enable drag-and-drop support in Avalonia. |
| [Xaml.Behaviors.Interactions.DragAndDrop.DataGrid](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop.DataGrid) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.DragAndDrop.DataGrid.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop.DataGrid) | DataGrid-specific drag-and-drop helpers built on top of the drag-and-drop framework. |
| [Xaml.Behaviors.Interactions.Draggable](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Draggable) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Draggable.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Draggable) | Draggable element behaviors for moving controls around. |
| [Xaml.Behaviors.Interactions.Events](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Events) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Events.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Events) | Behaviors responding to Avalonia input and focus events. |
| [Xaml.Behaviors.Interactions.ReactiveUI](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.ReactiveUI) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.ReactiveUI.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.ReactiveUI) | Behaviors integrating ReactiveUI navigation patterns. |
| [Xaml.Behaviors.Interactions.Responsive](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Responsive) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Responsive.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Responsive) | Behaviors to assist with responsive and adaptive UI layouts. |
| [Xaml.Behaviors.Interactions.Scripting](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Scripting) | [![NuGet](https://img.shields.io/nuget/v/Xaml.Behaviors.Interactions.Scripting.svg)](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Scripting) | Execute C# scripts at runtime to add dynamic behavior. |

## Docs

[XAML Behaviors for Avalonia documentation.](https://wieslawsoltes.github.io/Xaml.Behaviors/)

## Logo

The logo features a stylized hexagon (representing a behavior/component) with a gradient in Avalonia-like colors (purple/violet) and a white lightning bolt (representing interactivity/triggers) in the center.

![Logo](logo.svg)

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/Xaml.Behaviors)

## License

XAML Behaviors is licensed under the [MIT license](LICENSE.TXT).
