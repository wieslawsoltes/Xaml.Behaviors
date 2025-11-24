# Typed EventArgs Action

The `[GenerateEventArgsAction]` attribute generates a strongly-typed action that consumes a single parameter (typically an `EventArgs`-derived type) and can optionally project members from the event args into styled properties for binding.

## Usage

Annotate a handler method that accepts one `EventArgs`-derived parameter.

```csharp
using Avalonia.Input;
using Xaml.Behaviors.SourceGenerators;

public partial class PointerHandler
{
    [GenerateEventArgsAction(Project = "Position,KeyModifiers")]
    public void OnPointerPressed(PointerPressedEventArgs args)
    {
        // Respond to pointer press
    }
}
```

The generator produces `OnPointerPressedEventArgsAction` in the same namespace. For assembly-level usage, the class name is prefixed with the target type name and stays in that target namespace.

### XAML Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.Handlers">
  <Border>
    <Interaction.Behaviors>
      <local:OnPointerPressedEventArgsAction />
    </Interaction.Behaviors>
  </Border>
</UserControl>
```

When the action executes (e.g., from an `EventTriggerBehavior`), it casts `parameter` to `PointerPressedEventArgs`, updates projected properties (`Position`, `KeyModifiers`), and calls your method.

## Options

- `UseDispatcher`: marshal the method invocation to `Dispatcher.UIThread.Post`.
- `Name`: override the generated class name.
- `Project`: comma/semicolon-separated list of event-args property names to expose as styled properties (e.g., `Project = "Position,KeyModifiers"`).

## Diagnostics

- Reuses existing action diagnostics: static/generic not supported (`XBG010`/`XBG008`), method not found/ambiguous (`XBG006`/`XBG007`), accessibility (`XBG014`).
- Projection-specific diagnostics: `XBG027` when a projected member is missing; `XBG028` when the projected member exists but is not accessible.

## Notes

- Methods must have exactly one accessible parameter (commonly an `EventArgs`-derived type).
- Projected properties are populated from the event args before invocation, enabling binding to the latest event data.  
