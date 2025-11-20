# XAML Behaviors for Avalonia

**XAML Behaviors for Avalonia** is an easy-to-use library for adding common and reusable interactivity to your [Avalonia](https://github.com/AvaloniaUI/Avalonia) applications with minimal code. It is a port of the [Windows UWP XAML Behaviors](https://github.com/Microsoft/XamlBehaviors) library.

## Getting Started

### Installation

The library is available as a NuGet package. You can install it using the .NET CLI or the Package Manager Console.

**Package Manager Console:**

```powershell
Install-Package Xaml.Behaviors
```

**.NET CLI:**

```bash
dotnet add package Xaml.Behaviors
```

### Usage Example

Here is a simple example of how to use `EventTriggerBehavior` to invoke a command when a button is clicked:

```xml
<Button Content="Click Me">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <InvokeCommandAction Command="{Binding MyCommand}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

## Available Packages

The library is split into several packages to allow you to include only what you need:

| Package | Description |
|---------|-------------|
| **[Xaml.Behaviors](https://www.nuget.org/packages/Xaml.Behaviors)** | Meta-package that includes all the main behavior packages. Recommended for most users. |
| **[Xaml.Behaviors.Avalonia](https://www.nuget.org/packages/Xaml.Behaviors.Avalonia)** | Meta package that bundles all Avalonia XAML Behaviors for easy installation. |
| **[Xaml.Behaviors.Interactivity](https://www.nuget.org/packages/Xaml.Behaviors.Interactivity)** | The core foundation library providing base classes for Actions, Triggers, and Behaviors. |
| **[Xaml.Behaviors.Interactions](https://www.nuget.org/packages/Xaml.Behaviors.Interactions)** | Contains common actions and triggers like `CallMethodAction`, `ChangePropertyAction`, `EventTriggerBehavior`, etc. |
| **[Xaml.Behaviors.Interactions.Custom](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Custom)** | A collection of custom behaviors and actions for specific Avalonia controls and scenarios. |
| **[Xaml.Behaviors.Interactions.DragAndDrop](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop)** | Behaviors to enable Drag and Drop functionality. |
| **[Xaml.Behaviors.Interactions.DragAndDrop.DataGrid](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.DragAndDrop.DataGrid)** | DataGrid-specific drag-and-drop helpers built on top of the drag-and-drop framework. |
| **[Xaml.Behaviors.Interactions.Draggable](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Draggable)** | Draggable element behaviors for moving controls around. |
| **[Xaml.Behaviors.Interactions.Events](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Events)** | Behaviors for handling various input events (Pointer, Keyboard, Gestures). |
| **[Xaml.Behaviors.Interactions.ReactiveUI](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.ReactiveUI)** | Behaviors integrating ReactiveUI navigation patterns. |
| **[Xaml.Behaviors.Interactions.Responsive](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Responsive)** | Behaviors to assist with responsive and adaptive UI layouts. |
| **[Xaml.Behaviors.Interactions.Scripting](https://www.nuget.org/packages/Xaml.Behaviors.Interactions.Scripting)** | Execute C# scripts at runtime to add dynamic behavior. |

## Documentation Sections

*   **[Articles](articles/intro.md)**: In-depth guides and tutorials on how to use the various behaviors, actions, and triggers.
*   **[API Documentation](api/)**: Detailed technical documentation for all classes and members.

## License

XAML Behaviors for Avalonia is licensed under the [MIT License](https://github.com/wieslawsoltes/Xaml.Behaviors/blob/master/LICENSE.TXT).
