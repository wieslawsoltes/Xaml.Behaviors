# Animations sample application (Uno Platform)

The Uno Platform (Skia desktop) twin of [samples/AnimationsTestApplication](../../AnimationsTestApplication/README.md).
It demonstrates `Xaml.Behaviors.Uno.Animations` without referencing `Xaml.Behaviors.Uno.Interactivity` or any
interactions package.

The sample shares the C# sources of the Avalonia application (the demo controls under `Controls/` and the code-behind
of the views, compiled with the `UNO` symbol, see [samples/Uno/README.md](../README.md)). Every Avalonia view has a
WinUI twin at the same relative path (`Views/MainWindow.xaml`, `Views/MainView.xaml`, `Views/Pages/*.xaml`,
`SideBar.xaml`); `App.xaml(.cs)` and `Platforms/Desktop/Program.cs` are the Uno-only composition root and desktop head.

The left-hand `SingleSelectionTabControl` (a WinUI `TabView` with the vertical `SideBarTabControlStyle` template) lists
the same 16 pages:

- key-frame factory, builder, and synchronous/asynchronous runners;
- fluid movement with automatic transform preparation;
- fade, sliding, scale, and rotation primitives;
- attention, entrance, exit, Framer Motion, and special catalogs;
- selection indicator animation;
- orbit, tilt, and parallax composition effects;
- transition collection mutation and observation operations.

## Differences from the Avalonia sample

| Avalonia | Uno Platform |
|----------|--------------|
| `TabControl`/`TabItem` | `TabView`/`TabViewItem` without add and close buttons (`App.xaml`); the sidebar uses a vertical template (`SideBar.xaml`). |
| `Animation` (key frames) | WinUI `Storyboard` (the library's `PlatformAnimation`). |
| Composition offsets, orientation | `Microsoft.UI.Composition`; offsets are relative to the layout position, tilt and orbit rotate around an axis. |
| `SelectingItemsControlBehavior` on a `TabControl` | Enabled on the `TabView`'s tab strip selector (`TabListView`) of a page-local template whose tab items have a `PART_SelectedPipe` indicator; the indicator movement is an explicit key frame animation (Uno has no implicit animations). |
| `DoubleTransition` added through `TransitionOperations` | WinUI transition collections only hold theme transitions: an `EntranceThemeTransition` is added, observed and removed, and the opacity change is animated with a storyboard (`UIElement.OpacityTransition` is not implemented on Uno Platform). The page explains the difference. |
| `ClipToBounds` | A non-scrolling `ScrollViewer` clips the animated content. |
| `WrapPanel` `ItemSpacing`/`LineSpacing` | Item margins of half the spacing. |
| Attached/detached from the visual tree | `Loaded`/`Unloaded`. |
| Window size (1100x760) | Set through `AppWindow.Resize` in `App.xaml.cs`. |

Run it from the repository root:

```bash
dotnet run --project samples/Uno/AnimationsTestApplication/AnimationsTestApplication.csproj
```
