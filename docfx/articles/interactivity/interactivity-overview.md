# Interactivity Overview

The `Xaml.Behaviors.Interactivity` package provides a framework for adding interactivity to Avalonia applications using the Behaviors SDK pattern. This pattern allows you to encapsulate interactive behavior in reusable components that can be attached to controls in XAML.

## Core Concepts

The Interactivity SDK is built around three main concepts:

*   **Behaviors**: Encapsulate state and behavior that can be attached to an object.
*   **Triggers**: Listen for events or conditions and invoke actions.
*   **Actions**: Perform an operation when invoked by a trigger.

## The Interaction Class

The `Interaction` class is the entry point for using behaviors in XAML. It defines the `Behaviors` attached property, which allows you to attach a collection of behaviors to any `AvaloniaObject`.

### Attaching Behaviors in XAML

To attach behaviors to a control, you use the `Interaction.Behaviors` attached property.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Button Content="Click Me">
        <Interaction.Behaviors>
            <EventTriggerBehavior EventName="Click">
                <ChangePropertyAction TargetObject="{Binding #MyTextBlock}" 
                                      PropertyName="Text"
                                      Value="Button Clicked!" />
            </EventTriggerBehavior>
        </Interaction.Behaviors>
    </Button>

    <TextBlock Name="MyTextBlock" Text="Waiting..." />
</UserControl>
```

### Programmatic Access

You can also access and modify behaviors programmatically using the `Interaction.GetBehaviors` and `Interaction.SetBehaviors` methods.

```csharp
var behaviors = Interaction.GetBehaviors(myButton);
behaviors.Add(new MyCustomBehavior());
```

## Base Classes

The SDK provides several base classes to help you create custom interactive components:

*   **`Behavior` / `Behavior<T>`**: The base class for behaviors.
*   **`Trigger` / `TriggerBase<T>`**: The base class for triggers.
*   **`Action` / `TriggerAction<T>`**: The base class for actions.
*   **`StyledElementBehavior` / `StyledElementTrigger` / `StyledElementAction`**: Specialized base classes for behaviors that are attached to `StyledElement` objects (like Controls). These classes provide additional features like `DataContext` synchronization and logical tree attachment.

## Installation

To use the Interactivity SDK, you need to install the `Xaml.Behaviors` NuGet package.

```bash
dotnet add package Xaml.Behaviors
```
