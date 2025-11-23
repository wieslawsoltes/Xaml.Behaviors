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

Assembly-level attributes are supported as well. The `propertyName` parameter can be a literal name, a wildcard (`*`) pattern, or a regular expression. Classes generated from assembly attributes are prefixed with the target type to reduce collisions (e.g. `TextBlockSetTextAction`).

```csharp
[assembly: GenerateTypedChangePropertyAction(typeof(Avalonia.Controls.TextBlock), "Text")]
[assembly: GenerateTypedChangePropertyAction(typeof(MyApp.ViewModels.ShellViewModel), "*Property")]
[assembly: GenerateTypedChangePropertyAction(typeof(MyApp.ViewModels.ShellViewModel), "^(Status|Title)$")]
```

> Only public or internal properties with accessible setters are supported. Wildcard/regex assembly attributes ignore inaccessible matches; if no accessible properties match, a diagnostic is produced and no action class is generated.

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
