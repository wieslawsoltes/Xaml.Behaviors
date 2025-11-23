# Typed Triggers

The `[GenerateTypedTrigger]` attribute generates a strongly-typed `Trigger` class that subscribes to a specific CLR event on a source object. This is the AOT-safe alternative to `EventTriggerBehavior` for non-RoutedEvents.

## Usage

Annotate an event in your ViewModel (or any class) with `[GenerateTypedTrigger]`.

```csharp
public partial class MyViewModel
{
    [GenerateTypedTrigger]
    public event EventHandler? ProcessingFinished;
}
```

The generator will create a class named `ProcessingFinishedTrigger` in the same namespace.

You can also register triggers at the assembly level and generate multiple classes at once. The `eventName` argument accepts either a literal name, a wildcard pattern (e.g. `*Finished`), or a regular expression. When generated from an assembly attribute, the class name is prefixed with the target type to avoid collisions (e.g. `ButtonClickTrigger`).

```csharp
[assembly: GenerateTypedTrigger(typeof(Avalonia.Controls.Button), "Click")]                  // ButtonClickTrigger
[assembly: GenerateTypedTrigger(typeof(MyApp.ViewModels.ShellViewModel), "*Finished")]       // ShellViewModelProcessingFinishedTrigger, etc.
[assembly: GenerateTypedTrigger(typeof(MyApp.ViewModels.ShellViewModel), "^(Loaded|Closed)$")] // ShellViewModelLoadedTrigger, ShellViewModelClosedTrigger
```

> Only public or internal events visible to the generator are supported (internal requires being in the same/friend assembly). Wildcard/regex matches skip inaccessible events, and if no accessible matches are found a diagnostic will be emitted.

### Matching rules at a glance

| Aspect | Behavior |
| --- | --- |
| Pattern kinds | Literal name, `*` wildcard, or regular expression |
| Name collisions | Assembly attributes prefix the target type name (e.g. `ButtonClickTrigger`) |
| Accessibility | Events must be public/internal; the event handler delegate and its parameter types must be public |
| Ambiguity | Event name conflicts are resolved by the event name; duplicate matches are de-duplicated per name |
| Inaccessible matches | Wildcard/regex patterns drop matches using non-public members and emit XBG014 if nothing accessible remains |
| Static/generic | Static or generic events are rejected (diagnostic) |

### XAML Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.ViewModels">
    <!-- ... -->
    <local:ProcessingFinishedTrigger SourceObject="{Binding}">
        <local:SubmitAction />
    </local:ProcessingFinishedTrigger>
    <!-- ... -->
</UserControl>
```

## Weak Event Pattern

The generated trigger implements a **Weak Event Pattern**. This ensures that the trigger does not cause memory leaks if the Source Object lives longer than the Trigger (e.g., a singleton ViewModel and a transient View).

The trigger uses a proxy object to subscribe to the event. If the trigger is garbage collected, the proxy automatically unsubscribes from the source event the next time it is raised.
