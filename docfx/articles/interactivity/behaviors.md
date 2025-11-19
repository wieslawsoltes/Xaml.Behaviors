# Behaviors

Behaviors are reusable components that encapsulate functionality and can be attached to objects. They allow you to add behavior to controls without subclassing them or writing code-behind.

## Using Behaviors

Behaviors are attached to controls using the `Interaction.Behaviors` attached property. A control can have multiple behaviors attached to it.

```xml
<TextBox Text="Hello">
    <Interaction.Behaviors>
        <SelectAllOnGotFocusBehavior />
    </Interaction.Behaviors>
</TextBox>
```

## Creating Custom Behaviors

To create a custom behavior, you typically inherit from `Behavior<T>` or `StyledElementBehavior<T>`, where `T` is the type of object the behavior can be attached to.

### Behavior vs. StyledElementBehavior

*   **`Behavior<T>`**: Inherits from `AvaloniaObject`. It is the most basic behavior class. Use this if your behavior does not need to be part of the logical tree or share the `DataContext`.
*   **`StyledElementBehavior<T>`**: Inherits from `StyledElement`. It supports `DataContext` synchronization and is attached to the logical tree. This is the recommended base class for most behaviors that interact with UI controls.

### Example: specific Behavior

Here is an example of a simple behavior that logs a message when the associated button is clicked.

```csharp
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

public class LogClickBehavior : Behavior<Button>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Click += OnClick;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.Click -= OnClick;
    }

    private void OnClick(object? sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("Button clicked!");
    }
}
```

### Example: StyledElementBehavior

If your behavior needs to bind to properties in the `DataContext`, you should use `StyledElementBehavior`.

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

public class BindableBehavior : StyledElementBehavior<Control>
{
    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<BindableBehavior, string>(nameof(Message));

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        // ...
    }
}
```

## Lifecycle

The lifecycle of a behavior is tightly coupled with the lifecycle of the object it is attached to. The `Interaction` class manages this relationship by subscribing to various events on the `AssociatedObject` and propagating them to the attached behaviors.

### Why is this needed?

In many cases, a behavior needs to perform actions not just when it is attached, but also when the associated object is loaded into the visual tree, when its data context changes, or when it is unloaded. Without these lifecycle methods, you would have to manually subscribe to these events in `OnAttached` and unsubscribe in `OnDetaching` for every single behavior. The `Behavior` base classes handle this plumbing for you, exposing protected virtual methods that you can override.

### How it works

When you attach a behavior collection to a control, the `Interaction` class subscribes to relevant events on that control (like `AttachedToVisualTree`, `DetachedFromVisualTree`, `Loaded`, `Unloaded`, etc.). When these events fire, `Interaction` iterates through the behaviors in the collection and calls the corresponding lifecycle method on each behavior.

*   **`OnAttached`**: Called when the behavior is attached to an object. Use this method to subscribe to events or initialize the behavior.
*   **`OnDetaching`**: Called when the behavior is detached from an object. Use this method to unsubscribe from events and clean up resources.

### Additional Lifecycle Methods

The following methods are available for override to handle specific events from the associated object:

*   **`OnAttachedToVisualTree`**: Called after the associated object is attached to the visual tree.
*   **`OnDetachedFromVisualTree`**: Called when the associated object is being detached from the visual tree.
*   **`OnAttachedToLogicalTree`**: Called after the associated object is attached to the logical tree.
*   **`OnDetachedFromLogicalTree`**: Called when the associated object is being detached from the logical tree.
*   **`OnLoaded`**: Called after the associated object is loaded.
*   **`OnUnloaded`**: Called when the associated object is unloaded.
*   **`OnInitializedEvent`**: Called when the associated object is initialized.
*   **`OnDataContextChangedEvent`**: Called when the associated object's `DataContext` changes.
*   **`OnResourcesChangedEvent`**: Called when the associated object's `Resources` change.
*   **`OnActualThemeVariantChangedEvent`**: Called when the associated object's `ActualThemeVariant` changes.

## Enabling and Disabling

Behaviors have an `IsEnabled` property. When set to `false`, the behavior should disable its functionality. It is up to the behavior implementation to respect this property.

```csharp
public class MyBehavior : Behavior<Control>
{
    // ...

    private void OnSomeEvent(object sender, EventArgs e)
    {
        if (!IsEnabled) return;
        
        // Perform action
    }
}
```
