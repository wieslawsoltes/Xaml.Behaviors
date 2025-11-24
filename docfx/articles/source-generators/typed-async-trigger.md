# Typed Async Trigger

The `[GenerateAsyncTrigger]` attribute generates a trigger that reacts to `Task`/`ValueTask` properties without reflection or converter glue.

## Usage

```csharp
using System.Threading.Tasks;
using Xaml.Behaviors.SourceGenerators;

public partial class Loader
{
    [GenerateAsyncTrigger]
    public Task<int>? LoadTask { get; set; }
}
```

Generates `LoadTaskAsyncTrigger` with:
- `LoadTask` styled property to bind a `Task`/`Task<T>`/`ValueTask`/`ValueTask<T>`.
- `IsExecuting`, `LastError`, and `LastResult` (for `T`) styled properties.
- Executes actions when the task completes successfully; sets `LastError` on fault; cancels quietly without firing actions.

`UseDispatcher` defaults to `true` for callbacks. `FireOnAttach` controls whether the current task is tracked when the trigger attaches (set it to `false` to wait for a new value). `Name` can override the generated class name. Assembly-level attributes use the same defaults and generate prefixed class names under `Xaml.Behaviors.Generated`.

### XAML Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:Xaml.Behaviors.Generated">
  <Grid>
    <Interaction.Behaviors>
      <local:LoadTaskAsyncTrigger LoadTask="{Binding LoadTask}">
        <local:SetStatusTextAction Value="Loaded!" />
      </local:LoadTaskAsyncTrigger>
    </Interaction.Behaviors>
    <!-- UI content -->
  </Grid>
</UserControl>
```

## Diagnostics

- `XBG023` when the property name/pattern is missing.
- `XBG025` when the property type is not Task/Task<T>/ValueTask/ValueTask<T>.
- Generic/static members are rejected (`XBG008`/`XBG010`); accessibility issues report `XBG014`.

## Notes

- Assembly-level attributes generate classes in `Xaml.Behaviors.Generated` and prefix the target type name.
- Collisions are disambiguated via hashed suffixes.  
- `SourceObject` can be used to point the trigger at the owning object when the trigger property itself is not set (e.g., bind the trigger to the host control and let it read the task property from that source).
