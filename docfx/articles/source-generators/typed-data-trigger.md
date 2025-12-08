# Typed DataTrigger

The `[GenerateTypedDataTrigger]` attribute generates a strongly-typed `DataTriggerBehavior` class for a specific type. This avoids runtime type conversion using reflection, which is used by the standard `DataTriggerBehavior`.

## Usage

Add the attribute to your assembly (e.g., in `Program.cs` or `App.axaml.cs`).

```csharp
[assembly: GenerateTypedDataTrigger(typeof(double))]
[assembly: GenerateTypedDataTrigger(typeof(string))]
```

The generator will create classes named `DoubleDataTrigger` and `StringDataTrigger`.

### XAML Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:Xaml.Behaviors.Generated"
             xmlns:vm="using:MyApp.ViewModels">
    <!-- ... -->
    <Interaction.Behaviors>
        <local:DoubleDataTrigger Binding="{Binding Value}" Value="5" ComparisonCondition="Equal">
             <vm:SetStatusTextAction TargetObject="{Binding}" Value="Value is 5" />
        </local:DoubleDataTrigger>
    </Interaction.Behaviors>
    <!-- ... -->
</UserControl>
```

> Note: `DoubleDataTrigger` is generated in the `Xaml.Behaviors.Generated` namespace by default when using the assembly-level attribute. `SetStatusTextAction` is in your ViewModel's namespace.

In the generated trigger, the `Binding` and `Value` properties are strongly typed (e.g., `double`), so no type conversion is needed at runtime.
If the requested type is internal (and visible to the generator), the generated trigger class will be `internal`; otherwise it is `public`.
The generated trigger also evaluates once when attached, so matching initial values will fire actions immediately without waiting for another property change.

## Notes

- The generator intentionally scans attributes broadly (not only by metadata name) so aliased or currently-unresolved `GenerateTypedDataTrigger` usages are still discovered by the incremental pipeline.
