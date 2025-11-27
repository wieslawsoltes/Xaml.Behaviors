# Typed Event Command Trigger

The `[GenerateEventCommand]` attribute generates a trigger that subscribes to a CLR event and executes an `ICommand` when the event fires—no XAML boilerplate combining `EventTriggerBehavior` and `InvokeCommandAction`.

## Usage

Apply the attribute to an event (or at the assembly level).

```csharp
using Avalonia.Controls;
using Xaml.Behaviors.SourceGenerators;

[assembly: GenerateEventCommand(typeof(Button), "Click", ParameterPath = "Source", UseDispatcher = true)]
```

The generator creates `ClickEventCommandTrigger` (assembly scope keeps the target type’s namespace and prefixes the type name).

### XAML Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:controls="using:Avalonia.Controls">
  <Button Content="Click me">
    <Interaction.Behaviors>
      <controls:ButtonClickEventCommandTrigger Command="{Binding ClickedCommand}"
                                               ParameterPath="Source" />
    </Interaction.Behaviors>
  </Button>
  <!-- ClickedCommand lives on the DataContext; the trigger passes the event args Source as the command parameter. -->
</UserControl>
```

## Options

- `ParameterPath`: optional dotted path into event args (e.g., `OriginalSource`, `Key`) used as the command parameter. It is resolved at compile time into a typed accessor—no runtime reflection. The generated `ParameterPath` property must match the compile-time value; set it to `null`/empty to fall back to `Parameter` or the event args.
- `Parameter`: styled property to supply a fixed parameter (used if set and no path is provided).
- `UseDispatcher`: marshal `CanExecute/Execute` onto the UI thread.
- `Name`: optional override for the generated class name.
- `SourceObject`/`SourceName`: resolve the event source via binding or namescope lookup.
- Event delegates must use by-value parameters (`ref`/`in`/`out` parameters are not supported) and may have at most two parameters (the typical `sender`/`args` pattern).

## Diagnostics

- Reuses existing trigger diagnostics: `XBG004` (event not found), `XBG001-003` for unsupported delegates, plus accessibility checks.
- `XBG020`: `ParameterPath` does not resolve on the event args.
- `XBG021`: `ParameterPath` resolves to a non-public member; make it public or expose internals.

## Notes

- The trigger uses a weak proxy to detach if the source is GC’d.
- Parameter paths are compiled into direct member access to avoid reflection and trimming issues.
- Assembly-level generation prefixes the target type name to avoid collisions; hashed suffixes are used if multiple identical names are emitted.  
