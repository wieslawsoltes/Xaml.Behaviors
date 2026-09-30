# Xaml.PropertyGenerator

A Roslyn incremental source generator that writes XAML framework properties for you, so a single source file
compiles for **Avalonia** and for **WinUI / Uno Platform**. It was built to port Avalonia code to Uno Platform
without `#if` blocks around every property, and is independent from any other library.

```xml
<PackageReference Include="Xaml.PropertyGenerator" Version="x.y.z" PrivateAssets="all" />
```

Requirements: C# 14 (`<LangVersion>14</LangVersion>`, partial properties and the `field` keyword), .NET SDK 10.

## Usage

Declare a partial property and annotate it. The type must be partial (Uno Platform requires this for
`DependencyObject` types anyway).

```csharp
using Xaml.PropertyGenerator;

public partial class MyBehavior : Behavior
{
    /// <summary>Gets or sets the delay.</summary>
    [StyledProperty(DefaultValueExpression = "TimeSpan.FromMilliseconds(500)")]
    public partial TimeSpan Delay { get; set; }

    [StyledProperty(DefaultValue = true, DefaultBindingMode = PropertyBindingMode.TwoWay)]
    public partial bool IsActive { get; set; }

    [DirectProperty(DefaultValue = true)]
    public partial bool CanExecute { get; private set; }

    [DirectProperty(Lazy = true, Content = true)]
    public partial ActionCollection Actions { get; }

    // Optional: implement to be notified about changes on both platforms.
    partial void OnIsActiveChanged(bool oldValue, bool newValue) { }
}

[AttachedProperty("Tag", typeof(string), IsNullable = true, HostType = typeof(Control))]
public partial class Tags
{
}
```

| Attribute | Avalonia | WinUI / Uno Platform |
|-----------|----------|----------------------|
| `[StyledProperty]` | `StyledProperty<T>` + `AvaloniaProperty.Register` | `DependencyProperty.Register` |
| `[DirectProperty]` | `DirectProperty<TOwner, T>` + `field` backed accessors using `SetAndRaise` | `DependencyProperty` kept in sync with a `field` backing store |
| `[DirectProperty(Lazy = true)]` | `get => field ??= new T()` | value created on first access and stored in the dependency property (so it inherits the data context) |
| `[AttachedProperty]` | `AttachedProperty<T>` + `Get{Name}`/`Set{Name}` | `DependencyProperty.RegisterAttached` + `Get{Name}`/`Set{Name}` |

Every property also gets a `{Name}Property` identifier field with the accessibility of the property.

### Options

| Option | Notes |
|--------|-------|
| `DefaultValue` | Any attribute constant (enum values, `typeof`, primitives, strings). |
| `DefaultValueExpression` | C# expression for non-constant defaults. It is evaluated in the generated file, which repeats the `using` directives of the declaring file. |
| `DefaultBindingMode` | Avalonia only. WinUI has no default binding mode per property (reported as info `XPG0005`). |
| `Inherits` | Avalonia only (value inheritance). |
| `Content` | Avalonia `[Content]` on the property, WinUI `[ContentProperty(Name = ...)]` on the class. |
| `ResolveByName`, `AssignBinding` | Avalonia only. |
| `Lazy` (direct) | Get-only property created with its public parameterless constructor. |

### Change notifications

* `partial void On{Name}Changed(T oldValue, T newValue)` (instance properties) and
  `static partial void On{Name}Changed(THost element, T oldValue, T newValue)` (attached properties) are called on
  both platforms when you implement them.
* On WinUI, if the type or a base type declares an accessible
  `OnPropertyChanged(DependencyPropertyChangedEventArgs)` method, every generated property routes its changes to
  it. This mirrors Avalonia's `OnPropertyChanged(AvaloniaPropertyChangedEventArgs)` override so shared change
  handling keeps working.

### Platform selection

The platform is detected from the base type (`Avalonia.AvaloniaObject` or `Microsoft.UI.Xaml.DependencyObject`)
and falls back to the referenced assemblies. Override it with the MSBuild property
`<XamlPropertyGeneratorPlatform>Avalonia|WinUI</XamlPropertyGeneratorPlatform>`.

The attributes are generated into your compilation as internal, embedded and conditional types: they are never
visible to other assemblies (even with `InternalsVisibleTo`) and never persisted in metadata.

## Diagnostics

### XPG0001
The annotated property is not a `partial` property declaration without accessor bodies or initializer.

### XPG0002
The containing type (or one of its containing types) is not `partial`.

### XPG0003
Neither Avalonia nor WinUI/Uno Platform is referenced and `XamlPropertyGeneratorPlatform` is not set.

### XPG0004
Unsupported property shape: static or indexer properties, a lazy direct property with a setter or without a public
parameterless constructor, an Avalonia attached property owned by a static class, or a default value that cannot
be expressed as a constant (use `DefaultValueExpression`).

### XPG0005
Informational: an option that only exists on Avalonia (`Inherits`, `DefaultBindingMode`, `ResolveByName`,
`AssignBinding`) is ignored on WinUI.

## Migrating existing Avalonia code

The package also contains analyzer **XPG1001** with a code fix that converts hand-written registrations into
generated properties:

```csharp
// Before
public static readonly StyledProperty<bool> IsActiveProperty =
    AvaloniaProperty.Register<Owner, bool>(nameof(IsActive), defaultValue: true);

public bool IsActive
{
    get => GetValue(IsActiveProperty);
    set => SetValue(IsActiveProperty, value);
}

// After
[StyledProperty(DefaultValue = true)]
public partial bool IsActive { get; set; }
```

### XPG1001
Reported (severity *Info*) only when the conversion preserves the public API and behavior:

* `Register` / `RegisterDirect` owned by the containing type, named after the CLR property, with the same
  accessibility as the CLR property;
* only `defaultValue`, `inherits`, `defaultBindingMode` (and `enableDataValidation: false`) options;
* accessors that only forward to `GetValue`/`SetValue` (casts allowed), or for direct properties
  `get => _field; set => SetAndRaise(...)` and lazy `get => _field ??= new()`/`[]`;
* backing fields that are not used anywhere else;
* `[Content]`, `[ResolveByName]` and `[AssignBinding]` are folded into the attribute.

Apply it to a whole solution deterministically (the fix-all provider processes each document in one pass):

```bash
dotnet format analyzers MySolution.slnx --diagnostics XPG1001 --severity info
```

The fix adds `partial` to the containing types and a `using Xaml.PropertyGenerator;` directive when the namespace
is not already imported (for example through a global using). Preprocessor blocks that belonged only to the removed
field are removed with it; unbalanced directives are kept.

## License

MIT
