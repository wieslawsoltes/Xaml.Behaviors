# Typed ChangePropertyAction

The `[GenerateTypedChangePropertyAction]` attribute generates a strongly-typed `Action` class that sets a specific property on a target object. This is the AOT-safe alternative to `ChangePropertyAction`.

## Usage

Annotate a property in your ViewModel (or any class) with `[GenerateTypedChangePropertyAction]`.

```csharp
public partial class MyViewModel
{
    [GenerateTypedChangePropertyAction]
    public string StatusText { get; set; }
}
```

The generator will create a class named `SetStatusTextAction` in the same namespace.

### XAML Usage

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.ViewModels">
    <!-- ... -->
    <local:SetStatusTextAction TargetObject="{Binding}" Value="New Status" />
    <!-- ... -->
</UserControl>
```
