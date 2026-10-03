# WinUI 3 (Windows App SDK)

XAML Behaviors is also available for native **WinUI 3** applications (Windows App SDK). The WinUI packages are built
from exactly the sources of the [Uno Platform](../uno-platform/index.md) packages, which are themselves built from the
Avalonia sources: behaviors, triggers and actions keep their names, and the namespaces are the Uno Platform ones
(`Xaml.Interactivity`, `Xaml.Interactions.Core`, `Xaml.Interactions.Custom`, ...), so XAML written for one of the two
WinUI platforms works on the other.

> [!NOTE]
> The WinUI port is in progress. See [Differences from Uno Platform and Tracked Issues](winui-differences.md) for the
> current status.

| Package | Uno Platform twin |
|---------|-------------------|
| `Xaml.Behaviors.WinUI.Interactivity` | `Xaml.Behaviors.Uno.Interactivity` |
| `Xaml.Behaviors.WinUI.Interactions` | `Xaml.Behaviors.Uno.Interactions` |
| `Xaml.Behaviors.WinUI.Interactions.Events` | `Xaml.Behaviors.Uno.Interactions.Events` |
| `Xaml.Behaviors.WinUI.Interactions.Custom` | `Xaml.Behaviors.Uno.Interactions.Custom` |
| `Xaml.Behaviors.WinUI.Interactions.Responsive` | `Xaml.Behaviors.Uno.Interactions.Responsive` |
| `Xaml.Behaviors.WinUI.Interactions.Draggable` | `Xaml.Behaviors.Uno.Interactions.Draggable` |
| `Xaml.Behaviors.WinUI.Interactions.DragAndDrop` (+ `.DataGrid`) | `Xaml.Behaviors.Uno.Interactions.DragAndDrop` (+ `.DataGrid`) |
| `Xaml.Behaviors.WinUI.Animations` | `Xaml.Behaviors.Uno.Animations` |
| `Xaml.Behaviors.WinUI.Interactions.ReactiveUI` | `Xaml.Behaviors.Uno.Interactions.ReactiveUI` |
| `Xaml.Behaviors.WinUI.Interactions.Scripting` | `Xaml.Behaviors.Uno.Interactions.Scripting` |
| `Xaml.Behaviors.WinUI` / `Xaml.Behaviors.WinUI.All` | `Xaml.Behaviors.Uno` / `Xaml.Behaviors.Uno.All` |
| `Xaml.Behaviors.WinUI.Testing` (+ `.XUnit`) | `Xaml.Behaviors.Uno.Headless` (+ `.XUnit`) |

The packages target `net10.0-windows10.0.19041.0` and depend on `Microsoft.WindowsAppSDK`.

## Setting up an application

WinUI cannot enumerate the windows of an application. Behaviors and actions that work with the window of their element
(`CloseWindowAction`, `WindowStateTrigger`, the storage, clipboard and launcher actions, ...) find it among the windows
registered with `WindowTracker`:

```csharp
protected override void OnLaunched(LaunchActivatedEventArgs args)
{
    var window = new MainWindow();
    Xaml.Interactivity.WindowTracker.Track(window); // until the window is closed
    window.Activate();
}
```

```xml
<Page xmlns:i="using:Xaml.Interactivity"
      xmlns:ic="using:Xaml.Interactions.Core">
  <Button Content="Save">
    <i:Interaction.Behaviors>
      <ic:EventTriggerBehavior EventName="Click">
        <ic:InvokeCommandAction Command="{x:Bind ViewModel.SaveCommand}" />
      </ic:EventTriggerBehavior>
    </i:Interaction.Behaviors>
  </Button>
</Page>
```

## Repository layout

| Folder | Contents |
|--------|----------|
| `src/WinUI` | The WinUI libraries and the WinUI test harness. Every library source links its Uno Platform twin (`src/Uno/<Project>`); the few native WinUI replacements live next to the WinUI project files. |
| `tests/WinUI` | The WinUI test projects: every one source links its Uno Platform twin (`tests/Uno/<Project>`) and runs on the WinUI test harness. |
| `samples/WinUI` | The WinUI sample applications, source linking the Uno Platform samples (`samples/Uno/<Sample>`). |
| `WinUIBehaviors.slnx` | The WinUI solution (builds on Windows). |

The `UNO` symbol marks the WinUI API code shared by both ports; the `WINUI` symbol marks the native WinUI differences.
