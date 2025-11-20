# Scripting Overview

The `Xaml.Behaviors.Interactions.Scripting` package provides the ability to execute C# scripts directly from XAML in response to events. This is powered by the Roslyn scripting API.

## Namespaces

The package contains the following namespace:

*   **`Avalonia.Xaml.Interactions.Scripting`**: Contains `ExecuteScriptAction`.

## Installation

```bash
dotnet add package Xaml.Behaviors.Interactions.Scripting
```

## Important Considerations

*   **Trimming**: This package relies on dynamic code generation and compilation, which is **not compatible with trimming**. If you are publishing your application with trimming enabled, this package may not work correctly.
*   **Performance**: Compiling and executing scripts at runtime has a performance cost. Use this for prototyping, dynamic behavior, or simple logic where full compilation is not required.
