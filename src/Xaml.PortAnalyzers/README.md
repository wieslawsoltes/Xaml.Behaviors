# Xaml.PortAnalyzers

Roslyn analyzers and code fixes for projects whose **Avalonia** sources are shared with a port to
**Uno Platform / WinUI** (or any other target) that compiles the same files with a preprocessor symbol such
as `UNO`.

The analyzers run inside the regular **Avalonia** build. They evaluate every `#if` region as if the port
symbol were defined, so code that would break the port build is reported early, without building the port.

| Rule | Severity | Code fix | Summary |
|------|----------|----------|---------|
| [XPORT001](#xport001) | Warning | Yes | Type deriving directly from a dependency object must be partial |
| [XPORT002](#xport002) | Warning | No | Platform-specific API is not guarded for the port target |
| [XPORT003](#xport003) | Warning | No | Unknown preprocessor symbol in platform condition |
| [XPORT004](#xport004) | Warning | Yes | Dependency property getter must cast GetValue result |
| [XPORT005](#xport005) | Warning | No | Port map file could not be read |

All rules use the `Portability` category.

## Installation

```xml
<ItemGroup>
  <PackageReference Include="Xaml.PortAnalyzers" Version="x.y.z" PrivateAssets="all" />
</ItemGroup>
```

The package is a development dependency: it contains no runtime assemblies, it does not flow to consumers of
your library, and it supports compilers from Roslyn 4.8 (Visual Studio 17.8 / .NET 8 SDK) onwards.

Using the analyzers from source (for example inside the repository that builds them) requires the MSBuild
integration to be imported explicitly:

```xml
<Import Project="path/to/src/Xaml.PortAnalyzers/buildTransitive/Xaml.PortAnalyzers.props" />
<ItemGroup>
  <ProjectReference Include="path/to/src/Xaml.PortAnalyzers/Xaml.PortAnalyzers.csproj"
                    OutputItemType="Analyzer" ReferenceOutputAssembly="false" PrivateAssets="all" />
  <!-- Optional: code fixes in the IDE. -->
  <ProjectReference Include="path/to/src/Xaml.PortAnalyzers.CodeFixes/Xaml.PortAnalyzers.CodeFixes.csproj"
                    OutputItemType="Analyzer" ReferenceOutputAssembly="false" PrivateAssets="all" />
</ItemGroup>
<Import Project="path/to/src/Xaml.PortAnalyzers/buildTransitive/Xaml.PortAnalyzers.targets" />
```

## Configuration

### MSBuild properties

Set the properties in the project file (or `Directory.Build.props`). The package surfaces them to the
analyzers through `CompilerVisibleProperty` items.

| Property | Default | Description |
|----------|---------|-------------|
| `XamlPortTargetSymbol` | `UNO` | Preprocessor symbol that marks the port target. |
| `XamlPortSharedSourceExcludes` | _(empty)_ | `;`-separated globs, relative to the project directory, of files that are **not** shared with the port. XPORT001, XPORT002 and XPORT004 skip these files. Supports `*` (inside a path segment), `?` (one character) and `**` (any number of segments). Matching is case-insensitive; `\` and `/` are equivalent. Line breaks are allowed. |
| `XamlPortKnownSymbols` | _(empty)_ | Extra preprocessor symbols accepted by XPORT003, separated by `;`, `,` or white space. `*` and `?` wildcards are supported. |

Example that mirrors the list of files excluded from an Uno project:

```xml
<PropertyGroup>
  <XamlPortTargetSymbol>UNO</XamlPortTargetSymbol>
  <XamlPortSharedSourceExcludes>
    AvaloniaObjectBehaviorsExtensions.cs;
    Templates/**;
    Events/Handlers/*EventHandler.cs
  </XamlPortSharedSourceExcludes>
  <XamlPortKnownSymbols>MY_FEATURE;LEGACY_*</XamlPortKnownSymbols>
</PropertyGroup>
<ItemGroup>
  <AdditionalFiles Include="uno.xamlport.json" />
</ItemGroup>
```

> The generated analyzer configuration treats `;` and `#` as the start of a comment. The package's
> `buildTransitive` targets therefore rewrite list values to a single line separated by `|` before the file is
> generated; the analyzers accept `;` and `|` (and `,` for symbols).

### .editorconfig keys

`.editorconfig` (or `.globalconfig`) keys take precedence over the MSBuild properties.

| Key | Description |
|-----|-------------|
| `xaml_port.shared = true\|false` | Forces a file to be treated as shared (or not shared) with the port, regardless of `XamlPortSharedSourceExcludes`. |
| `xaml_port.target_symbol = WINUI` | Overrides the port target symbol for the matching files. |
| `xaml_port.known_symbols = A, B*` | Adds known symbols for XPORT003. Use `,` or spaces as separators (`;` starts a comment in `.editorconfig`). |

```ini
[src/MyLib/Platform/**.cs]
xaml_port.shared = false

[*.cs]
dotnet_diagnostic.XPORT002.severity = error
```

## How code is classified

* **Shared file**: every C# file of the project that is not matched by `XamlPortSharedSourceExcludes` (or
  `xaml_port.shared = false`). Generated code is never analyzed.
* **Compiled for the port**: a node of a shared file is compiled for the port when all enclosing conditional
  directives would select it with the port target symbol defined. Conditions are evaluated with
  * the port target symbol **defined**,
  * all symbols of the current build (`DefineConstants`, e.g. `NET8_0_OR_GREATER`) **defined**,
  * every other symbol **undefined**,
  * `#define` / `#undef` directives of the file applied in order.

  `!`, `&&`, `||`, `==`, `!=`, parentheses, `true`/`false`, nested `#if` blocks and `#elif`/`#else` chains are
  supported. For example, with `UNO` as the target symbol:

  ```csharp
  #if !UNO
      // not compiled for the port: never reported
  #endif

  #if UNO
      // not compiled for Avalonia: the analyzers never see it
  #else
      // not compiled for the port: never reported
  #endif

  #if NET8_0_OR_GREATER || !UNO
      // compiled for the port when the current build defines NET8_0_OR_GREATER: reported
  #endif
  ```

### Directive evaluation algorithm

For each syntax tree the analyzers walk all directive trivia once, including directives nested in regions that
are inactive in the current build, and keep a stack of `#if` frames (`parent active`, `branch taken`).
`#if` pushes a frame, `#elif` takes its branch only if no previous branch was taken, `#else` takes the remaining
branch and `#endif` pops. After every directive the resulting "compiled for the port" state is recorded as a
transition at the end of the directive; a node is looked up with a binary search on its start position. The map
is computed lazily, once per tree and compilation, and shared by all callbacks.

## Port map (`*.xamlport.json`)

XPORT002 needs to know which source platform symbols exist on the port. This is described by one or more
port map files passed as `AdditionalFiles` whose name ends with `.xamlport.json`. Multiple files are merged.
Without a port map XPORT002 is silent. The files accept `//` and `/* */` comments.

A JSON schema is shipped in the package (`xamlport.schema.json`) and is available at
`https://raw.githubusercontent.com/wieslawsoltes/Xaml.Behaviors/master/src/Xaml.PortAnalyzers/xamlport.schema.json`.

```json
{
  "$schema": "https://raw.githubusercontent.com/wieslawsoltes/Xaml.Behaviors/master/src/Xaml.PortAnalyzers/xamlport.schema.json",
  "sourceNamespacePrefixes": ["Avalonia"],
  "mapped": {
    "types": ["Avalonia.AvaloniaObject", "Avalonia.Controls.Control"],
    "namespaces": ["Avalonia.Xaml.Interactivity"],
    "members": ["Avalonia.AvaloniaObject.GetValue", "Avalonia.AvaloniaObject.SetValue"]
  },
  "unsupported": {
    "types": [],
    "namespaces": ["Avalonia.LogicalTree"]
  },
  "renamedNamespaces": {
    "Avalonia.Xaml.Interactions": "Xaml.Interactions"
  }
}
```

| Property | Description |
|----------|-------------|
| `sourceNamespacePrefixes` | Only symbols whose namespace equals or is nested in one of these namespaces are checked. Defaults to `["Avalonia"]`. |
| `mapped.types` | Types that exist on the port under the same simple name (natively, through `global using` aliases or through compatibility shims). Names are fully qualified, without type arguments (`Avalonia.StyledProperty`) or with the arity suffix (``Avalonia.StyledProperty`1``). Nested types use `.` (`Outer.Inner`). Enum members of a mapped enum are mapped too. |
| `mapped.namespaces` | Namespaces that exist on the port with the same name. All types and members inside them (and inside nested namespaces) are mapped and `using` them is allowed. |
| `mapped.members` | Members that exist on the port: `Namespace.Type.Member` (all overloads) or `Namespace.Type.*` (all members of the type). A mapped type does **not** map its members: list the ones the port provides. Members are identified by the type that declares them, e.g. `Avalonia.AvaloniaObject.GetValue`; extension methods by their static class, e.g. `Avalonia.AvaloniaObjectExtensions.GetObservable`. |
| `unsupported.types` / `namespaces` / `members` | Symbols that are known not to exist on the port. They take precedence over `mapped` and produce a more specific message. An unsupported type makes its members and nested types unsupported. |
| `renamedNamespaces` | Namespaces whose types are ported under another namespace (`"old": "new"`). Types and members inside them are mapped, but `using` directives and namespace qualifiers that spell the old name are reported. |

Symbols declared in **shared source files of the current project** are always treated as mapped (they are
compiled for the port too). Symbols declared only in excluded files are checked against the port map like
any other symbol.

## Rules

### XPORT001

**Type deriving directly from a dependency object must be partial**

On Uno Platform `DependencyObject` is an interface whose implementation is generated as another `partial`
declaration of every class deriving directly from it. A shared class whose base type is
`Avalonia.AvaloniaObject` (the Avalonia counterpart), whose base type is `Microsoft.UI.Xaml.DependencyObject`, or
which lists the `Microsoft.UI.Xaml.DependencyObject` interface directly, must be `partial`, together with all its
containing types. Otherwise the port fails with CS0260.

```csharp
public class MyBehavior : AvaloniaObject { }          // XPORT001
public partial class MyBehavior : AvaloniaObject { }  // OK
```

The code fix adds `partial` to the type and to its containing types.

### XPORT002

**Platform-specific API is not guarded for the port target**

Reported in shared files for code that is compiled for the port (see [above](#how-code-is-classified)) when a
reference to a type, member, `using` directive, `using static` / alias target or namespace qualifier names a
source platform symbol that the port map

* declares as `unsupported`,
* does not declare as `mapped` (for namespaces in `using` directives and qualifiers: not in `mapped.namespaces`),
* declares as renamed (`using` directives and namespace qualifiers only).

```csharp
using Avalonia.LogicalTree;                    // XPORT002: 'Avalonia.LogicalTree' is unsupported

public partial class MyBehavior : AvaloniaObject
{
    public double Width(Control c) => c.Bounds.Width;   // XPORT002: 'Avalonia.Visual.Bounds' is not mapped
}
```

Fix it by guarding the code, or by mapping the symbol once the port provides it:

```csharp
#if !UNO
using Avalonia.LogicalTree;
#endif

public partial class MyBehavior : AvaloniaObject
{
#if UNO
    public double Width(Control c) => c.ActualWidth;
#else
    public double Width(Control c) => c.Bounds.Width;
#endif
}
```

Documentation comments (`cref`) are not checked.

### XPORT003

**Unknown preprocessor symbol in platform condition**

Reported for every identifier in an `#if` or `#elif` condition (of any file, shared or not, including nested
conditions in inactive regions) that is neither defined in the current build, nor defined by `#define` / `#undef`
in the file, nor a known symbol. Such identifiers are usually typos that silently exclude code:

```csharp
#if UNOO        // XPORT003
#endif
#if !UNO_       // XPORT003
#endif
```

Known symbols: `DEBUG`, `RELEASE`, `TRACE`, `NET*`, `NETSTANDARD*`, `NETCOREAPP*`, `NETFRAMEWORK*`,
`*_OR_GREATER`, `DOCFX`, `UNO`, `HAS_UNO*`, `__UNO*`, `__ANDROID__`, `__IOS__`, `__MACOS__`,
`__MACCATALYST__`, `__WASM__`, `__SKIA__`, `__TVOS__`, `WINAPPSDK`, `WINDOWS_UWP`, `WINUI`, `AVALONIA`,
`WINDOWS`, `ANDROID`, `IOS`, `MACCATALYST`, `MACOS`, `TVOS`, `BROWSER`, `WASI`, `LINUX`, `OSX`, `FREEBSD`, the
port target symbol and `XamlPortKnownSymbols`. Note that wildcard entries such as `NET*` also accept typos that
start with the same prefix.

### XPORT004

**Dependency property getter must cast GetValue result**

Avalonia's `GetValue<T>(StyledProperty<T>)` returns `T`, while WinUI's `GetValue(DependencyProperty)` returns
`object`. In shared files, a property getter (expression-bodied property, `get =>` accessor or `return`
statement in a `get` block) whose returned expression is exactly a `GetValue(...)` / `x.GetValue(...)` call
(optionally parenthesized or null-forgiven) must cast the result, unless the property type is `object`.

```csharp
public bool IsEnabled
{
    get => GetValue(IsEnabledProperty);        // XPORT004
    set => SetValue(IsEnabledProperty, value);
}

public bool IsEnabled
{
    get => (bool)GetValue(IsEnabledProperty);  // OK (the cast is redundant but harmless on Avalonia)
    set => SetValue(IsEnabledProperty, value);
}
```

The code fix inserts a cast to the declared property type.

### XPORT005

**Port map file could not be read**

A `*.xamlport.json` file is not valid JSON or does not follow the schema (unknown properties are rejected to
catch typos). The file is ignored and the diagnostic points at the offending location.

## Suppressing diagnostics

* In code: `#pragma warning disable XPORT002` / `#pragma warning restore XPORT002`, or
  `[SuppressMessage("Portability", "XPORT002")]`.
* Per file or folder: `dotnet_diagnostic.XPORT002.severity = none` in `.editorconfig`, or
  `xaml_port.shared = false` to treat the files as not shared.
* Per project: `<NoWarn>$(NoWarn);XPORT003</NoWarn>`.
* Prefer extending the port map (XPORT002) or `XamlPortKnownSymbols` (XPORT003) over suppressions.

## Performance

The analyzers register per-compilation state in `RegisterCompilationStartAction`, enable concurrent execution,
skip generated code, cache per-tree settings and directive maps, cache symbol classifications, avoid LINQ in
callbacks and do not use reflection. XPORT002 only binds names when a port map is present and the name is in a
shared file and compiled for the port.
