# Uno Platform

XAML Behaviors is also available for [Uno Platform](https://platform.uno) (WinUI API). The Uno Platform packages are
built from the same sources as the Avalonia packages, so behaviors, triggers and actions keep their names and
behavior; namespaces drop the `Avalonia.` prefix (`Avalonia.Xaml.Interactivity` → `Xaml.Interactivity`).

| Package | Contents |
|---------|----------|
| `Xaml.Behaviors.Uno.Interactivity` | `Behavior`, `Trigger`, `Action`, `Interaction`, collections, conditions |
| `Xaml.Behaviors.Uno.Interactions` | Core triggers and actions, clipboard, file and folder pickers, file system and network |
| `Xaml.Behaviors.Uno.Interactions.Events` | Input and focus event triggers |
| `Xaml.Behaviors.Uno.Interactions.Custom` | Custom behaviors, triggers, actions and converters |
| `Xaml.Behaviors.Uno.Interactions.Responsive` | Adaptive and aspect ratio behaviors (style classes map to visual states) |
| `Xaml.Behaviors.Uno.Interactions.Draggable` | Drag behaviors |
| `Xaml.Behaviors.Uno.Interactions.DragAndDrop` (+ `.DataGrid`) | Drag and drop behaviors |
| `Xaml.Behaviors.Uno.Animations` | Storyboard, composition and transition helpers |
| `Xaml.Behaviors.Uno.Interactions.ReactiveUI` | ReactiveUI navigation and interaction behaviors (ReactiveUI 25.1) |
| `Xaml.Behaviors.Uno.Interactions.Scripting` | C# scripting actions |
| `Xaml.Behaviors.Uno` / `Xaml.Behaviors.Uno.All` | Single assembly / meta package |

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

The repository documents the port in more detail:

* `src/Uno/README.md` — packages, usage and the behavior differences from Avalonia.
* `src/Uno/PORTING.md` — how the sources are shared and the status of every project.
* `tests/Uno/README.md` and `samples/Uno/README.md` — the shared tests and the Uno twins of the sample applications.
* `src/Uno/Xaml.Behaviors.Uno.Headless/README.md` — headless Uno Platform UI tests with xUnit v3.

See [Known Issues and Differences](known-issues.md) for the open issues, the issues fixed during the port and the
WinUI differences to keep in mind.
