# AOT-Friendly Behaviors with Source Generators

## Overview

`Xaml.Behaviors` provides a set of Source Generators to create strongly-typed, AOT-friendly alternatives to the traditional reflection-based behaviors.

Traditional behaviors like `CallMethodAction`, `ChangePropertyAction`, and `DataTriggerBehavior` rely heavily on `System.Reflection` to resolve methods and properties at runtime. This approach has several drawbacks:
*   **Performance**: Reflection is slower than direct method calls.
*   **AOT Compatibility**: Reflection requires metadata that might be trimmed by the linker in AOT (Ahead-Of-Time) compilation scenarios.
*   **Type Safety**: Errors (e.g., typos in method names) are only discovered at runtime.

The Source Generators solve this by generating specific `Action` and `Trigger` classes at compile time that directly invoke methods or access properties, ensuring full AOT compatibility and better performance.

## Getting Started

1.  Add the `Xaml.Behaviors.SourceGenerators` package to your project.
2.  Use the provided attributes to annotate your ViewModels or Assemblies.
3.  Use the generated classes in your XAML.

## Available Generators

*   **[Typed Actions](typed-action.md)**: Generate actions that call methods on your ViewModel. Supports parameter binding and async methods.
*   **[Typed Triggers](typed-trigger.md)**: Generate triggers that subscribe to CLR events on your ViewModel.
*   **[Typed ChangePropertyAction](typed-change-property.md)**: Generate actions that set specific properties.
*   **[Typed DataTrigger](typed-data-trigger.md)**: Generate strongly-typed data triggers to avoid runtime type conversion.
*   **[Typed MultiDataTrigger](typed-multi-data-trigger.md)**: Generate triggers that evaluate multiple conditions with zero allocation.
*   **[Typed InvokeCommandAction](typed-invoke-command-action.md)**: Generate strongly-typed command actions.
*   **[Typed Event Command](typed-event-command.md)**: Generate event-to-command triggers without XAML boilerplate.
*   **[Typed Property Trigger](typed-property-trigger.md)**: Generate triggers that listen to Avalonia properties without bindings.
*   **[Typed EventArgs Action](typed-event-args-action.md)**: Generate actions that consume specific event args with optional member projection.
*   **[Typed Async Trigger](typed-async-trigger.md)**: Generate triggers that react to tasks without converters or reflection.
*   **[Typed Observable Trigger](typed-observable-trigger.md)**: Generate triggers that react to observables without converters or reflection.
*   **[Diagnostics](diagnostics.md)**: Understand analyzer errors (XBG001-XBG032) with fixes and examples.

## Example

Instead of:

```xml
<CallMethodAction TargetObject="{Binding}" MethodName="Submit" />
```

You can use:

```csharp
[GenerateTypedAction]
public void Submit() { ... }
```

And in XAML:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MyApp.ViewModels">
  <!-- ... -->
  <local:SubmitAction TargetObject="{Binding}" />
  <!-- ... -->
</UserControl>
```

> **Note:** In the examples below, `local:` refers to the namespace where your generated classes are located (e.g., your ViewModel namespace), and `ac:` refers to `Avalonia.Controls` or other standard namespaces. Ensure you define these XML namespaces in your XAML file.
