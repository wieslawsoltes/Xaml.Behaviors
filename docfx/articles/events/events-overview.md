# Events Package Overview

The `Xaml.Behaviors.Interactions.Events` package provides a comprehensive set of strongly-typed **Behaviors** and **Triggers** for common Avalonia events. These components allow you to handle input events (Pointer, Keyboard, Focus, Gestures) directly in XAML with precise control over routing strategies.

## Key Features

*   **Strongly Typed Wrappers**: Dedicated classes for specific events (e.g., `PointerPressedEventTrigger`, `KeyDownEventBehavior`) eliminate the need to type event names as strings.
*   **Routing Strategies**: Exposes the `RoutingStrategies` property (Tunnel, Bubble, Direct), allowing you to capture events during the tunneling phase or handle bubbling events, which is often difficult with standard event triggers.
*   **Base Classes**: Provides abstract base behaviors (e.g., `PointerPressedEventBehavior`) that you can inherit from to create reusable, event-specific logic without boilerplate event subscription code.

## Available Events

The package covers a wide range of Avalonia events, categorized below:

### Pointer Events
*   `PointerPressed`
*   `PointerReleased`
*   `PointerMoved`
*   `PointerEntered`
*   `PointerExited`
*   `PointerWheelChanged`
*   `PointerCaptureLost`

### Gestures & Taps
*   `Tapped`
*   `DoubleTapped`
*   `RightTapped`
*   `ScrollGesture`
*   `ScrollGestureEnded`

### Keyboard & Input
*   `KeyDown`
*   `KeyUp`
*   `TextInput`
*   `TextInputMethodClientRequested`

### Focus
*   `GotFocus`
*   `LostFocus`

### Drag & Drop (Raw Events)
*   `DragEnter`
*   `DragLeave`
*   `DragOver`
*   `Drop`

> **Note**: For high-level Drag & Drop functionality (like reordering items), consider using the `Xaml.Behaviors.Interactions.DragAndDrop` package. The triggers listed here are for handling the raw low-level drag events.

## Usage Examples

### 1. Using an Event Trigger

The most common use case is to trigger an Action when an event occurs. The `RoutingStrategies` property is particularly useful here.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <StackPanel Spacing="10">
    <Border Background="LightBlue" Height="100" Width="200">
      <Interaction.Behaviors>
        <!-- Trigger on the Tunneling phase (Preview) -->
        <PointerPressedEventTrigger RoutingStrategies="Tunnel">
          <ChangePropertyAction TargetObject="{Binding #StatusText}" 
                                   PropertyName="Text" 
                                   Value="Tunnel: Pointer Pressed!" />
        </PointerPressedEventTrigger>
      </Interaction.Behaviors>
      <TextBlock Text="Click Me (Tunnel)" HorizontalAlignment="Center" VerticalAlignment="Center"/>
    </Border>

    <TextBlock Name="StatusText" Text="Waiting..." />
  </StackPanel>
</UserControl>
```

### 2. Creating a Custom Behavior

If you need to execute code logic rather than XAML Actions, you can inherit from the abstract `*EventBehavior` classes. This handles the `AddHandler` and `RemoveHandler` logic for you.

```csharp
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Events;

public class LogPositionBehavior : PointerPressedEventBehavior
{
    protected override void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var position = e.GetPosition(AssociatedObject);
        System.Diagnostics.Debug.WriteLine($"Clicked at: {position}");
        
        // You can also mark the event as handled if needed
        // e.Handled = true;
    }
}
```

Usage in XAML:

```xml
<Border Background="LightGreen" Height="100" Width="200">
  <Interaction.Behaviors>
    <local:LogPositionBehavior RoutingStrategies="Bubble" />
  </Interaction.Behaviors>
  <TextBlock Text="Check Debug Output" HorizontalAlignment="Center" VerticalAlignment="Center"/>
</Border>
```

### 3. PointerEventsBehavior

The `PointerEventsBehavior` is a special abstract base class that subscribes to three events simultaneously: `PointerPressed`, `PointerReleased`, and `PointerMoved`. This is useful for implementing complex interactions like custom dragging or drawing behaviors.

```csharp
public class CustomDrawingBehavior : PointerEventsBehavior
{
    protected override void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Start drawing
    }

    protected override void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        // Continue drawing
    }

    protected override void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // Stop drawing
    }
}
```
