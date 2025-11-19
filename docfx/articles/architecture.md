# Architecture

Understanding the internal architecture of `Xaml.Behaviors` helps in writing custom behaviors and debugging complex interactions. The library is built upon the concept of **Attached Properties** and a specific lifecycle management system.

## Core Components

### 1. The `Interaction` Class

The `Interaction` class is the entry point for the entire system. It defines the `Behaviors` attached property, which is the mechanism used to attach a collection of behaviors to an `AvaloniaObject` (typically a `Control`).

```xml
<Button>
    <Interaction.Behaviors>
        <!-- Behaviors go here -->
    </Interaction.Behaviors>
</Button>
```

When the XAML parser encounters `<Interaction.Behaviors>`, it calls `Interaction.GetBehaviors()`, which initializes a `BehaviorCollection` for that control.

### 2. BehaviorCollection

This is a specialized collection that holds `IBehavior` instances. When a behavior is added to this collection, the collection notifies the `Interaction` system, which then initiates the attachment process.

### 3. The `Behavior` Class

The `Behavior` (and `Behavior<T>`) class is the base class for all interactive components. It maintains a reference to the object it is attached to, known as the `AssociatedObject`.

## The Attachment Lifecycle

One of the most critical aspects of the architecture is how and when behaviors are attached to controls. Unlike simple property setting, behaviors have a lifecycle that often mirrors the visual tree lifecycle of the control.

### Attachment Process

1.  **Collection Initialization**: The `BehaviorCollection` is created and assigned to the `Interaction.Behaviors` attached property of the target control.
2.  **Visual Tree Observation**: The `Interaction` class observes the `AttachedToVisualTree` and `DetachedFromVisualTree` events of the target control.
3.  **Attach**:
    *   When the behavior is added to the collection *and* the control is in the visual tree, `Behavior.Attach(target)` is called.
    *   This invokes the protected `OnAttached()` method.
    *   **Developer Responsibility**: In `OnAttached()`, you should subscribe to events, initialize properties, or start services.
4.  **Detach**:
    *   When the control is removed from the visual tree, or the behavior is removed from the collection, `Behavior.Detach()` is called.
    *   This invokes the protected `OnDetaching()` method.
    *   **Developer Responsibility**: In `OnDetaching()`, you **must** unsubscribe from events and dispose of resources to prevent memory leaks.

### Why Visual Tree Events?

Waiting for the control to be attached to the visual tree ensures that the control is fully initialized and its templates are applied. This is crucial for behaviors that need to traverse the visual tree or access template parts.

## Triggers and Actions

The architecture separates the "cause" (Trigger) from the "effect" (Action).

*   **Triggers** (e.g., `EventTriggerBehavior`) listen for something to happen. They derive from `Behavior` and use the same lifecycle.
*   **Actions** (e.g., `InvokeCommandAction`) are executed by Triggers. They do not have a persistent lifecycle in the same way; they are simply `Execute`d.

## Custom Behaviors

When creating custom behaviors, you inherit from `Behavior<T>`. The architecture handles the plumbing, leaving you to implement just two methods:

```csharp
public class MyCustomBehavior : Behavior<Button>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        // Access AssociatedObject here
        AssociatedObject.Click += OnClick;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        // Clean up
        AssociatedObject.Click -= OnClick;
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
        // Handle click
    }
}
```
