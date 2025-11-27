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
- Executes actions when the task completes successfully, passing the result as the action parameter for `Task<T>/ValueTask<T>`; sets `LastError` on fault; cancellation stops tracking without firing actions.

`UseDispatcher` defaults to `true` for callbacks. `FireOnAttach` controls whether the current task is tracked when the trigger attaches (set it to `false` to wait for a new value). `Name` can override the generated class name. Assembly-level attributes use the same defaults, emit in the target type’s namespace, and prefix the type name to avoid collisions.

### XAML Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.ViewModels">
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

- `XBG023` when no Task/ValueTask property matching the name/pattern is found on the target type.
- `XBG025` when the property type is not Task/Task<T>/ValueTask/ValueTask<T>.
- Generic/static members are rejected (`XBG008`/`XBG010`); accessibility issues report `XBG014`.

## Notes

- Assembly-level attributes stay in the target type’s namespace and prefix the type name.
- Collisions are disambiguated via hashed suffixes.  
- `SourceObject` can be used to point the trigger at the owning object when the trigger property itself is not set (e.g., bind the trigger to the host control and let it read the task property from that source).
