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
