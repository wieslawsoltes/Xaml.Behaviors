# Typed Observable Trigger

The `[GenerateObservableTrigger]` attribute generates a trigger that reacts to `IObservable<T>` properties without reflection or converter glue.

## Usage

```csharp
using System;
using Xaml.Behaviors.SourceGenerators;

public partial class StreamHost
{
    [GenerateObservableTrigger]
    public IObservable<int>? Stream { get; set; }
}
```

Generates `StreamObservableTrigger` with:
- `Stream` styled property to bind an `IObservable<T>`.
- `LastValue` and `LastError` styled properties.
- Executes actions on `OnNext` (value passed as parameter), `OnError` (exception), and `OnCompleted` (null).

`UseDispatcher` defaults to `true`; `FireOnAttach` subscribes immediately when the trigger attaches (set it to `false` to wait for a property change after attach). `Name` can override the generated class name. Assembly-level attributes use the same defaults and emit prefixed types in `Xaml.Behaviors.Generated`.

### XAML Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:Xaml.Behaviors.Generated">
  <Grid>
    <Interaction.Behaviors>
      <local:StreamObservableTrigger Stream="{Binding Stream}">
        <local:SetStatusTextAction Value="New value received" />
      </local:StreamObservableTrigger>
    </Interaction.Behaviors>
    <!-- UI content -->
  </Grid>
</UserControl>
```

## Diagnostics

- `XBG024` when the property name/pattern is missing.
- `XBG026` when the property type is not `IObservable<T>`.
- Generic/static members are rejected (`XBG008`/`XBG010`); accessibility issues report `XBG014`.

## Notes

- Assembly-level attributes generate classes in `Xaml.Behaviors.Generated` and prefix the target type name.
- Collisions are disambiguated via hashed suffixes.  
